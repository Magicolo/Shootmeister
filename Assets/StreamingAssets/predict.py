import numpy
from tensorflow import keras

directory = './Assets/StreamingAssets/'
model: keras.Model = keras.models.load_model(directory + 'model')


def predict(features):
    x = numpy.array([features])
    y = model.predict(x)[0]
    return list(y)
