# rabbitmq-profiler
An user-friendly option to the default rabbitmq-firehose.

### Requirements
1. Enable the RabbitMQ.Tracing Plugin 
```sh
rabbitmq-plugins enable rabbitmq_tracing
```
2. Enable the tracing via RabbitMQ Control.
```sh
rabbitmqctl trace_on
```