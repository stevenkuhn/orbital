# Orbital
Orbital is an ASP.NET MVC site that displays graphs of Windows Performance Monitor metrics.

___This contains rough concept/prototype code at the moment.___

## PerfMon Assumptions
This site assumes that you are already storing your PerfMon metrics into a SQL database. You have to create a System DSN source in the _ODBC Data Source Administrator_ before you can create a DataCollector that records the metrics to the database.