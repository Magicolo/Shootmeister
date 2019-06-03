import io
import numpy
import tensorflow
from tensorflow import keras
from tensorflow import data


def parse(line: str):
    left, right = line.split(':')
    features, labels = [], []
    for value in left.split(','):
        features.append(float(value))
    for value in right.split(','):
        labels.append(int(value))
    return features, labels


directory = './Assets/StreamingAssets/'
x = []
y = []
with open(directory + 'data') as wrapper:
    for line in wrapper.readlines():
        features, labels = parse(line)
        if features.count(0) < len(features):
            x.append(features)
            y.append(labels)

x = numpy.array(x)
y = numpy.array(y)
neurons = 64
dropout = 0.8
model = keras.Sequential([
    keras.layers.Dense(neurons, activation=keras.activations.relu),
    keras.layers.Dropout(dropout),
    keras.layers.Dense(neurons * 2, activation=keras.activations.relu),
    keras.layers.Dropout(dropout),
    keras.layers.Dense(neurons * 4, activation=keras.activations.relu),
    keras.layers.Dropout(dropout),
    keras.layers.Dense(neurons * 2, activation=keras.activations.relu),
    keras.layers.Dropout(dropout),
    keras.layers.Dense(neurons, activation=keras.activations.relu),
    keras.layers.Dropout(dropout),
    keras.layers.Dense(3, activation=keras.activations.sigmoid),
])

model.compile(
    optimizer=keras.optimizers.RMSprop(),
    loss=keras.losses.categorical_crossentropy,
    metrics=[
        keras.metrics.mean_squared_error,
        keras.metrics.binary_accuracy,
        keras.metrics.categorical_accuracy,
    ]
)

log = keras.callbacks.TensorBoard(log_dir=directory + "log")
model.fit(x, y, epochs=3, validation_split=0.1, callbacks=[log])
keras.models.save_model(model, directory + 'model')

print(x.shape, x.size, x.dtype)
print(y.shape, y.size, y.dtype)
