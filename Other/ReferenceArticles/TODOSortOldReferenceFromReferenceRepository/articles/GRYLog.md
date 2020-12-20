# GRYLog

`GRYLog` is an easy-to-use logger for any kind of logs. `GRYLog` uses the Microsoft-[LogLevel](https://docs.microsoft.com/de-de/dotnet/api/microsoft.extensions.logging.loglevel) and has currently 3 targets for logs-entries:
- Console
- Logfile
- Windows-Event-Log (only on Windows)

There are many configuration-possibilities for `GRYLog` like
- DateTime-Format
- Log errors as information (useful for programs which write "normal" outputs to StdErr)
- Whether times should be converted to UTC or not
- Whether stacktraces of logged exceptions should also be logged
- Whether empty lines can be logged
- Which Log-entry-Target logs which `LogLevel`

It is also possible to store this configuration in a configuration-file and change it at runtime. `GRYLog` can watch its configuration-file and reload it on change. So if you have a strange behavior in your productive program which you can not easily debug then you can at least "increase" the LogLevel (for example to `Debug`) and set that also stack-traces of exception should be logged.
