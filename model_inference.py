import torch
import torch.nn as nn
import numpy as np


# 1. Объявление той же архитектуры модели
class EEGAutoencoder(nn.Module):
    def __init__(self):
        super().__init__()
        self.encoder = nn.Sequential(
            nn.Conv1d(1, 16, kernel_size=3, stride=2, padding=1),
            nn.ReLU(),
            nn.Conv1d(16, 32, kernel_size=3, stride=2, padding=1),
            nn.ReLU()
        )
        self.decoder = nn.Sequential(
            nn.ConvTranspose1d(32, 16, kernel_size=3, stride=2, padding=1, output_padding=1),
            nn.ReLU(),
            nn.ConvTranspose1d(16, 1, kernel_size=3, stride=2, padding=1, output_padding=1),
            nn.Tanh()
        )

    def forward(self, x):
        return self.decoder(self.encoder(x))


# 2. Загрузка сохраненной модели и метаданных
checkpoint = torch.load('eeg_model.pth')

model = EEGAutoencoder()
model.load_state_dict(checkpoint['model_state'])
model.eval()  # Отключает вычисление градиентов и спец. слои (режим инференса)

MEAN = checkpoint['mean']
STD = checkpoint['std']
THRESHOLD = checkpoint['threshold']


def predict_window(raw_sensor_data):
    """
    Принимает: raw_sensor_data — список или 1D NumPy-массив из 128 чисел с Ардуино.
    Возвращает: (is_anomaly: bool, mse_score: float)
    """
    norm_data = (np.array(raw_sensor_data, dtype=np.float32) - MEAN) / (STD + 1e-8)

    tensor_input = torch.tensor(norm_data).unsqueeze(0).unsqueeze(0)

    with torch.no_grad():
        reconstructed = model(tensor_input)
        mse_error = torch.mean((tensor_input - reconstructed) ** 2).item()

    is_anomaly = mse_error > THRESHOLD
    return is_anomaly, mse_error


if __name__ == "__main__":
    # Симуляция поступления одного окна данных из 128 точек
    sample_window = [0.12, -0.05, 0.31, 0.85, 0.41, -0.10] + [0.0] * 122

    is_anomaly, score = predict_window(sample_window)

    print(f"MSE ошибка окна: {score:.6f} | Установленный порог: {THRESHOLD:.6f}")
    if is_anomaly:
        print("[ТРЕВОГА] Обнаружена аномалия в сигнале ЭЭГ!")
    else:
        print("[ОК] Сигнал соответствует норме.")