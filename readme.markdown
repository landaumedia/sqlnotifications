![](https://ci.appveyor.com/api/projects/status/u68ha9an9px5gdjt?svg=true)

# Sql Notifications

Projekt dient dazu eine Überwachung auf Datenbankebene zu aktivieren.


## Tracker
Derzeit unterstützte Tracker:<br>
### timestamp
 überprüft anhand des Timestamps auf Änderungen und trackt bekannte Ids. Dadurch kann er Inserts und Updates unterscheiden.
 (unterstützt: OnInsert, OnUpdate, AddionalFields)
### changeonlytimestamp
 überprüft anhand des Timestamps auf Änderungen. Inserts werden auch als OnUpdate getriggert was aber schneller ist.   (unterstützt: OnUpdate, AddionalFields) 
### changetracking
 benutzt die ChangteTracking FUnktion des SQLServer.(unterstützt: OnInsert, OnUpdate, OnDelete)




