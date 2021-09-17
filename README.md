# RabbitMQUserWebapi
Firstly postgresql create database with users table and (id username password) fields.
Then run 'docker-compose up' in docker file directory to create RabbitMQ and UserApi services.
Then run Console app to check RabbitMQ messages.
Lastly run UserAuthentification program.
P.S. It is better to open 2 cmd consoles and run 'dotnet run' command in RabbitMQ.Consumer directory,
then run 'dotnet run' command in UserAuthentification directory to check and control both logs.
