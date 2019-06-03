import os
import clr
import random
from tensorflow import keras

directory = os.getcwd() + '/'
clr.AddReference(directory + '../Game/Game.dll')

from Game import Environment
from System import Array
from System import Double

step_count = 1000
action_count = Environment.Actions
observation_count = Environment.Observations


def run():
    rewards = Environment.Reward()
    rewards.Shoot = -0.01
    rewards.Step = 0.1
    rewards.Death = -5
    environment = Environment(step_count, rewards)
    environment.Initialize()

    done = False
    while not done:
        actions = [random.random() for _ in range(action_count)]
        result = environment.Step(actions)
        done, observations, reward = result.Item1, [_ for _ in result.Item2], result.Item3
        print(done, observations, reward)

    environment.Dispose()


run()

# neurons = 64
# dropout = 0.8
# model = keras.Sequential([
#     keras.layers.Dense(neurons, activation=keras.activations.relu),
#     keras.layers.Dropout(dropout),
#     keras.layers.Dense(neurons * 2, activation=keras.activations.relu),
#     keras.layers.Dropout(dropout),
#     keras.layers.Dense(neurons * 4, activation=keras.activations.relu),
#     keras.layers.Dropout(dropout),
#     keras.layers.Dense(neurons * 2, activation=keras.activations.relu),
#     keras.layers.Dropout(dropout),
#     keras.layers.Dense(neurons, activation=keras.activations.relu),
#     keras.layers.Dropout(dropout),
#     keras.layers.Dense(3, activation=keras.activations.sigmoid),
# ])

# model.compile(
#     optimizer=keras.optimizers.Adam(),
#     loss=keras.losses.categorical_crossentropy,
#     metrics=[
#         keras.metrics.mean_squared_error,
#         keras.metrics.binary_accuracy,
#         keras.metrics.categorical_accuracy,
#     ]
# )

# log = keras.callbacks.TensorBoard(log_dir=directory + "log")
# model.fit(x, y, epochs=3, validation_split=0.1, callbacks=[log])
# keras.models.save_model(model, directory + 'model')
