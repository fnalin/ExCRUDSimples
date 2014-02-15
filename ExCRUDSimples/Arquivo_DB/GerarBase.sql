/* 
		******************************** ***************************************
		Script criado por Fabiano Nalin em 6/11/13 às 0:00h

		Script para criar a Base de Dados

*/

USE master

GO
IF(SELECT Count(name) FROM sys.databases WHERE name = 'CadEmpresaContatos')=1
	BEGIN
		ALTER DATABASE CadEmpresaContatos SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
		DROP DATABASE CadEmpresaContatos;
	END
GO

--Criando DATABASE CadEmpresaContatos
CREATE DATABASE CadEmpresaContatos ON  PRIMARY 
( NAME = N'CadEmpresaContatos', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\CadEmpresaContatos.mdf' , 
	SIZE = 10MB , FILEGROWTH = 100%)
 LOG ON 
( NAME = N'CadEmpresaContatos_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\CadEmpresaContatos_log.ldf' ,
	 SIZE = 1MB , FILEGROWTH = 100%)
GO
USE CadEmpresaContatos;
GO

ALTER DATABASE CadEmpresaContatos SET RECOVERY SIMPLE WITH NO_WAIT;

CREATE TABLE tbDemoCrudEmpresa
(
	ID				INT				IDENTITY(1,1),
	CNPJ			VARCHAR(20)		NOT NULL,
	Nome			VARCHAR(60)		NOT NULL,
	NomeFantasia	VARCHAR(60)		NOT NULL,
	WebSite			VARCHAR(60)

	CONSTRAINT PK_tbDemoCrudEmpresa_ID PRIMARY KEY(ID)
);
INSERT INTO tbDemoCrudEmpresa(CNPJ, Nome, NomeFantasia, WebSite)
	VALUES
		('00.000.000/0000-00','Nalin Corporation','Nalin Corp','http://www.nalincorp.net'),
		('99.999.999/9999-99','Marvel Ltda','Marvel','http://www.marvel.com'),
		('11.111.111/1111-11','Vaquinha Dalva SA','Dalvinha','http://www.vaquinhadalva.edu.br');

--SELECT * FROM tbDemoCrudEmpresa;

CREATE TABLE tbDemoCrudContato
(
	ID				INT				IDENTITY(1,1),
	EmpresaID		INT				NOT NULL,
	Nome			VARCHAR(60)		NOT NULL,
	Telefone		VARCHAR(14), 
	Celular			VARCHAR(14),
	Email			VARCHAR(60)

	CONSTRAINT PK_tbDemoCrudContato_ID PRIMARY KEY(ID)
	CONSTRAINT FK_tbDemoCrudContato_EmpresaID FOREIGN KEY(EmpresaID) REFERENCES tbDemoCrudEmpresa(ID)
);

INSERT INTO tbDemoCrudContato(EmpresaID, Nome, Telefone, Celular, Email)
	VALUES
		(1,'Fulano da Silva', '(11)5555-5555', '(11)69999-9999', 'fulaninho@nalincorp.net'),
		(1,'Ciclano Souza', '(11)2222-2222', '(11)68888-9999', 'ciclaninho@nalincorp.net'),
		(1,'Beltrano Pinto', '(11)5555-0001', '(11)68777-9999', 'beltraninho@nalincorp.net'),
		(2,'Pafunciano Silva', '(12)5555-1001', '(12)68777-9999', 'pafun@marvel.com'),
		(2,'Feliciano Pinto', '(12)5555-1002', '(12)68766-9999', 'fefe@marvel.com'),
		(3,'Jacinto Madruga', '(19)5555-6666', '(12)61111-1999', 'madruga@vaquinhadalva.edu.br'),
		(3,'Jacinto Madruga Jr', '(19)5555-6667', '(12)61111-1111', 'madruguinha@vaquinhadalva.edu.br');

--SELECT * FROM tbDemoCrudContato;
