-- ================================
-- Database: firmyDB
-- ================================
CREATE DATABASE firmyDB;
GO

USE firmyDB;
GO

-- ================================
-- TABLES
-- ================================

-- ================================
-- 1. Companies
-- ================================
CREATE TABLE Companies (
    Id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Code NVARCHAR(50) NOT NULL,
    LeaderId INT NULL
);

-- ================================
-- 2. Divisions
-- ================================
CREATE TABLE Divisions (
    Id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Code NVARCHAR(50) NOT NULL,
    CompanyId INT NOT NULL,
    LeaderId INT NULL,

    CONSTRAINT FK_Divisions_Companies
        FOREIGN KEY (CompanyId)
        REFERENCES Companies(Id)
        ON DELETE CASCADE
);

-- ================================
-- 3. Projects
-- ================================
CREATE TABLE Projects (
    Id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Code NVARCHAR(50) NOT NULL,
    DivisionId INT NOT NULL,
    LeaderId INT NULL,

    CONSTRAINT FK_Projects_Divisions
        FOREIGN KEY (DivisionId)
        REFERENCES Divisions(Id)
        ON DELETE CASCADE
);

-- ================================
-- 4. Departments
-- ================================
CREATE TABLE Departments (
    Id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Code NVARCHAR(50) NOT NULL,
    ProjectId INT NOT NULL,
    LeaderId INT NULL,

    CONSTRAINT FK_Departments_Projects
        FOREIGN KEY (ProjectId)
        REFERENCES Projects(Id)
        ON DELETE CASCADE
);

-- 5. Employees
CREATE TABLE Employees (
    Id INT IDENTITY PRIMARY KEY,
    Title NVARCHAR(50),
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(30),
    Email NVARCHAR(200) NOT NULL,
    CompanyId INT NOT NULL,
    DepartmentId INT NULL,

    CONSTRAINT FK_Employees_Companies
        FOREIGN KEY (CompanyId)
        REFERENCES Companies(Id)
        ON DELETE No ACTION,

    CONSTRAINT FK_Employees_Departments
        FOREIGN KEY (DepartmentId)
        REFERENCES Departments(Id)
        ON DELETE SET NULL
);

-- ================================
-- FOREIGN KEY CONSTRAINTS FOR LEADERS
-- ================================
ALTER TABLE Companies
ADD CONSTRAINT FK_Companies_Employees_LeaderId
FOREIGN KEY (LeaderId)
REFERENCES Employees(Id)
ON DELETE NO ACTION;

ALTER TABLE Divisions
ADD CONSTRAINT FK_Divisions_Employees_LeaderId
FOREIGN KEY (LeaderId)
REFERENCES Employees(Id)
ON DELETE NO ACTION;

ALTER TABLE Projects
ADD CONSTRAINT FK_Projects_Employees_LeaderId
FOREIGN KEY (LeaderId)
REFERENCES Employees(Id)
ON DELETE NO ACTION;

ALTER TABLE Departments
ADD CONSTRAINT FK_Departments_Employees_LeaderId
FOREIGN KEY (LeaderId)
REFERENCES Employees(Id)
ON DELETE NO ACTION;


-- ================================
-- SEED DATA
-- ================================

-- Company
INSERT INTO Companies (Name, Code)
VALUES ('Company', 'SF001');

-- Division
INSERT INTO Divisions (Name, Code, CompanyId)
VALUES ('Development', 'DIV01', 1);
-- Project
INSERT INTO Projects (Name, Code, DivisionId)
VALUES ('Project Alpha', 'P001', 1);

-- Department
INSERT INTO Departments (Name, Code, ProjectId)
VALUES ('Frontend', 'DEP01', 1);

-- Employees
INSERT INTO Employees (Title, FirstName, LastName, Phone, Email, CompanyId, DepartmentId)
VALUES 
('Ing.', 'John', 'Smith', '0900123456', 'john.smith@example.com', 1, 1),
('Mgr.', 'Marry', 'Smith', '0900654321', 'marry.smith@example.com', 1, 1),
('Bc.', 'Peter', 'Smith', '0900987654', 'peter.smith@example.com', 1, 1),
('Mgr.', 'Anna', 'Smith', '0900112233', 'anna.smith@example.com', 1, 1);

-- Assign Leaders
UPDATE Companies
SET LeaderId = 1
WHERE Id = 1;

UPDATE Divisions
SET LeaderId = 2 
WHERE Id = 1;

UPDATE Projects
SET LeaderId = 3
WHERE Id = 1;

UPDATE Departments
SET LeaderId = 4 
WHERE Id = 1;