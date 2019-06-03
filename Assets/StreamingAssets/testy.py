import clr
import os

directory = os.path.dirname(__file__)
plugins = os.path.join(os.getcwd(), '../Plugins/Entia/')
clr.AddReference(plugins + 'Entia.Core.dll')
clr.AddReference(plugins + 'Entia.dll')

from Entia import World
from Entia.Modules import WorldExtensions as Modules
import Entia.Components as Components
import Entia.Messages as Messages
world = World()
entities = Modules.Entities(world)
messages = Modules.Messages(world)

on_create = messages.Receive[Messages.OnCreate]()
entity = entities.Create()
message = Messages.OnCreate()
print(on_create.TryPop(message))
print(message.Entity)
on_create.Dispose()
