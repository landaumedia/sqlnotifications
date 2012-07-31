# Sql Notifications

Projekt dient dazu eine Überwachung auf Datenbankebene zu aktivieren.


## Tracker
Derzeit unterstützte Tracker:<br>
### timestamp
 überprüft anhand des Timestamps auf Änderungen und trackt bekannte Ids 
 (unterstützt: OnInsert, OnUpdate, AddionalFields)
### changeonlytimestamp
 überprüft anhand des Timestamps auf Änderungen   (unterstützt: OnUpdate, AddionalFields)
### changetracking
 benutzt die ChangteTracking FUnktion des SQLServer.(unterstützt: OnInsert, OnUpdate, OnDelete)




