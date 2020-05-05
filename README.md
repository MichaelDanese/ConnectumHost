# Host
#Funtional Requirements:
  -User account creation
  -Allow real-time communication
  -API that can interact with frontend, backend, and the database.
#Non Functional Requirements:
  -Security for users’ information and password
  -Resizing between desktop and mobile
  -Easy and fluid menus
#SignalR:
  -Allowed for real-time communication using hubs to establish connections
  -In this case the hub was used for one-to-one connections
#Backend:
  -Deal with application logic
  -Handle a Restful API
  -Handle security
#Security:
  -Salted and Hashed passwords
  -Implmented HTTPS
  -Used JWT for authentication
#Database:
  -Implemented Microsoft SQL Server
  -Generated databases with a code-first design using Entity Framework
  -Has tables for active connections, friend, users, and more.
