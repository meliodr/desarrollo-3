CREATE TABLE IF NOT EXISTS Partidos (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Fecha TEXT NOT NULL,
    Hora TEXT NOT NULL,
    EquipoLocal TEXT NOT NULL,
    EquipoVisitante TEXT NOT NULL,
    Estadio TEXT NOT NULL,
    TotalAsientos INTEGER NOT NULL DEFAULT 72
);

CREATE TABLE IF NOT EXISTS Registros (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Codigo TEXT NOT NULL UNIQUE,
    TipoDocumento TEXT NOT NULL,
    Documento TEXT NOT NULL,
    Nombres TEXT NOT NULL,
    Apellidos TEXT NOT NULL,
    FechaNacimiento TEXT NOT NULL,
    Sexo TEXT NOT NULL,
    Asiento TEXT NOT NULL,
    FechaRegistro TEXT NOT NULL,
    PartidoId INTEGER NOT NULL,
    FOREIGN KEY (PartidoId) REFERENCES Partidos(Id)
);

CREATE UNIQUE INDEX IF NOT EXISTS IX_Registros_PartidoId_Asiento
ON Registros (PartidoId, Asiento);

INSERT INTO Partidos (Fecha, Hora, EquipoLocal, EquipoVisitante, Estadio, TotalAsientos) VALUES
('2026-06-11', '19:00', 'México', 'Canadá', 'Estadio Azteca', 72),
('2026-06-12', '16:00', 'Estados Unidos', 'Gales', 'SoFi Stadium', 72),
('2026-06-12', '20:00', 'Argentina', 'Japón', 'MetLife Stadium', 72),
('2026-06-13', '17:00', 'Brasil', 'Marruecos', 'Hard Rock Stadium', 72),
('2026-06-13', '21:00', 'España', 'Senegal', 'AT&T Stadium', 72),
('2026-06-14', '18:00', 'Francia', 'Uruguay', 'BC Place', 72),
('2026-06-14', '22:00', 'Alemania', 'Corea del Sur', 'Levi''s Stadium', 72);
