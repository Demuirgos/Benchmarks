# Benchmarks
a bunch of experiments and benchmarks
Usage : 
```
cd ./Runner
./Run
```
Method : 
```
Feeder            Target             MMF-Sink            Observer
  |                 | |                 | |                 |
  | <===set-offset==| |                 | |                 |
  | ==send-data====>| |                 | |                 |
  |                 | |==report-stage==>| |                 |
  | ==send-data====>| |                 | |<===read-report==|
  |                 | |==report-stage==>| |                 |
  | ==send-data====>| |                 | |<===read-report==|
  |                 | |==report-stage==>| |                 |
  | ==send-data====>| |                 | |<===read-report==|
  |                 | |==report-stage==>| |                 |
Feeder            Target             MMF-Sink           Observer
```
