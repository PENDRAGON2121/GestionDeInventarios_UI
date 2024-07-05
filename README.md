
## SQL SERVER USER SCRIPT

Para Crear las tablas

```bash
CREATE TABLE Users (
    ID INT PRIMARY KEY IDENTITY(1,1),
    OauthID NVARCHAR(50) NULL,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Password NVARCHAR(100) NULL,
    Role INT NOT NULL,
    LoginAttempts INT DEFAULT 0,
    IsBlocked BIT DEFAULT 0,
    BlockedUntil DATETIME NULL
);

CREATE TABLE Inventarios (
    Id INT PRIMARY KEY IDENTITY,
    Nombre NVARCHAR(100),
    Categoria INT,
    Cantidad INT,
    Precio DECIMAL(10, 2),
);

CREATE TABLE HistorialDeInventario (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    InventarioId INT NOT NULL,
    Usuario NVARCHAR(100) NOT NULL,
    Fecha DATETIME NOT NULL,
    TipoModificacion INT NOT NULL
);

CREATE TABLE AjusteDeInventario (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Id_Inventario INT NOT NULL,
    CantidadActual INT NOT NULL,
    Ajuste INT NOT NULL,
    Tipo INT NOT NULL,
    Observaciones NVARCHAR(MAX) NOT NULL,
    UserId Nvarchar(255) NOT NULL,
    Fecha DATETIME NOT NULL
);

CREATE TABLE Caja (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId NVARCHAR(100) NOT NULL,
    FechaDeInicio DATETIME NOT NULL,
    FechaDeCierre DATETIME NULL,
    Estado INT NOT NULL
);

CREATE TABLE Ventas (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NombreCliente NVARCHAR(100) NOT NULL,
    Fecha DATETIME NOT NULL,
    TipoDePago INT NOT NULL,
    Total DECIMAL(18, 2) NOT NULL,
    SubTotal DECIMAL(18, 2) NOT NULL,
    PorcentajeDescuento INT NOT NULL,
    MontoDescuento DECIMAL(18, 2) NOT NULL,
    UserId NVARCHAR(100) NOT NULL,
    Estado INT NOT NULL,
    IdAperturaDeCaja INT NOT NULL
);

CREATE TABLE VentaDetalle (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Id_Venta INT NOT NULL,
    Id_Inventario INT NOT NULL,
    Cantidad INT NOT NULL,
    Precio DECIMAL(18, 2) NOT NULL,
    Monto DECIMAL(18, 2) NOT NULL,
    MontoDescuento DECIMAL(18, 2) NOT NULL,
);


INSERT INTO Users(OauthID, Name, Email, Password, Role, LoginAttempts, IsBlocked, BlockedUntil)
VALUES (NULL, 'Administrador', 'PPGR.GestorDeInventario.2024@gmail.com', 'Nuevo123*', 1, 0, 0, NULL);

  
```