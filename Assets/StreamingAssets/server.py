from clr.System.IO.Pipes import *

with NamedPipeServerStream('Poulah', PipeDirection.InOut, 1) as server:
    pass
    # buffer = []
    # server.Read(buffer, 0, 1024)
