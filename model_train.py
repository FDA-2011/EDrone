import numpy as np
import torch
import torch.nn as nn
from torch.utils.data import Dataset, DataLoader


class EEGFileDataset(Dataset):
    def __init__(self, file_path):
        if file_path.endswith('.npy'):
            raw_data = np.load(file_path).astype(np.float32)
        else:
            raw_data = np.loadtxt(file_path, delimiter=',').astype(np.float32)

        self.mean = float(raw_data.mean())
        self.std = float(raw_data.std())
        normalized = (raw_data - self.mean) / (self.std + 1e-8)

        self.data = torch.tensor(normalized).unsqueeze(1)

    def __len__(self):
        return len(self.data)

    def __getitem__(self, idx):
        return self.data[idx]


class EEGAutoencoder(nn.Module):
    def __init__(self):
        super().__init__()
        # Энкодер: сжимаем 128 отсчетов -> 64 -> 32
        self.encoder = nn.Sequential(
            nn.Conv1d(1, 16, kernel_size=3, stride=2, padding=1),
            nn.ReLU(),
            nn.Conv1d(16, 32, kernel_size=3, stride=2, padding=1),
            nn.ReLU()
        )
        # Декодер: восстанавливаем 32 отсчета -> 64 -> 128
        self.decoder = nn.Sequential(
            nn.ConvTranspose1d(32, 16, kernel_size=3, stride=2, padding=1, output_padding=1),
            nn.ReLU(),
            nn.ConvTranspose1d(16, 1, kernel_size=3, stride=2, padding=1, output_padding=1),
            nn.Tanh()
        )

    def forward(self, x):
        return self.decoder(self.encoder(x))


if __name__ == "__main__":
    dataset = EEGFileDataset('paht_to_file.csv')
    dataloader = DataLoader(dataset, batch_size=32, shuffle=True)

    model = EEGAutoencoder()
    criterion = nn.MSELoss()
    optimizer = torch.optim.Adam(model.parameters(), lr=0.001)

    model.train()
    epochs = 15
    for epoch in range(epochs):
        running_loss = 0.0
        for batch in dataloader:
            optimizer.zero_grad()
            outputs = model(batch)
            loss = criterion(outputs, batch)
            loss.backward()
            optimizer.step()
            running_loss += loss.item()
        print(f"Эпоха {epoch + 1}/{epochs} | Loss (MSE): {running_loss / len(dataloader):.6f}")

    model.eval()
    with torch.no_grad():
        all_data = dataset.data
        reconstructed = model(all_data)
        mse_errors = torch.mean((all_data - reconstructed) ** 2, dim=(1, 2)).numpy()

        threshold = float(mse_errors.mean() + 2 * mse_errors.std())

    checkpoint = {
        'model_state': model.state_dict(),
        'mean': dataset.mean,
        'std': dataset.std,
        'threshold': threshold
    }

    torch.save(checkpoint, 'eeg_model.pth')
    print(f"\n[УСПЕХ] Модель успешно сохранена в 'eeg_model.pth'")
    print(f"Порог аномальности: {threshold:.6f}")