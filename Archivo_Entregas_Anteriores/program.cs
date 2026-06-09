//creamos una base de datos. la guardamos como mydata.mdf
//Crear tabla. la primera es tblClientes. Id:int , TipoDocumento:nvarchar(50), 
//Documento:nvarchar(50), Nombre:nvarchar(150)
// Apellido:nvarchar(150)
//FechaNacimiento:datetime
//Estado: nvarchar(50)
//FechaIngreso:datetime
//notas:nvarchar(250)

//Insertar registro en nuestra base de datos. primero conectamos la base de datos. dentro del main colocamos.
//Sqlconnection sql  = new sqlconnection(@"Server=./Mydata.mdf");
//agregar sql.open(); al final cerrar la conexion colando sql.close();
//dentro de sql.open(); colcoamos.
//console.writeline(sql.state);
//sql.createcommand cmd = new sql.createcommand()
//cmd.comandtext = "insert into tblClientes(TipoDocumento, Documento, Nombre, Apellido, FechaNacimiento, Estado, FechaIngreso, notas) values('1', '12345678', 'Juan', 'Perez', '1990-01-01', 'Activo', '2022-01-01', 'Notas');";


