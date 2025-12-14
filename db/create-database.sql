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

-- Companies
INSERT INTO Companies (Name, Code) VALUES ('Company A', 'C001');
INSERT INTO Companies (Name, Code) VALUES ('Company B', 'C002');

-- Divisions (2 per company)
INSERT INTO Divisions (Name, Code, CompanyId) VALUES ('Dev Div A1', 'D001', 1);
INSERT INTO Divisions (Name, Code, CompanyId) VALUES ('QA Div A2', 'D002', 1);
INSERT INTO Divisions (Name, Code, CompanyId) VALUES ('Dev Div B1', 'D003', 2);
INSERT INTO Divisions (Name, Code, CompanyId) VALUES ('QA Div B2', 'D004', 2);

-- Projects (1 per division)
INSERT INTO Projects (Name, Code, DivisionId) VALUES ('Project A1', 'P001', 1);
INSERT INTO Projects (Name, Code, DivisionId) VALUES ('Project A2', 'P002', 2);
INSERT INTO Projects (Name, Code, DivisionId) VALUES ('Project B1', 'P003', 3);
INSERT INTO Projects (Name, Code, DivisionId) VALUES ('Project B2', 'P004', 4);

-- Departments (1 per project)
INSERT INTO Departments (Name, Code, ProjectId) VALUES ('Dept A1-1', 'DEP01', 1);
INSERT INTO Departments (Name, Code, ProjectId) VALUES ('Dept A2-1', 'DEP02', 2);
INSERT INTO Departments (Name, Code, ProjectId) VALUES ('Dept B1-1', 'DEP03', 3);
INSERT INTO Departments (Name, Code, ProjectId) VALUES ('Dept B2-1', 'DEP04', 4);

-- Employees (4 per company, rozdelení po 2 do oddelení)
-- Company A
INSERT INTO Employees (Title, FirstName, LastName, Phone, Email, CompanyId, DepartmentId) VALUES
('Mgr.', 'Alice', 'A', '0900123450', 'alice.a@example.com', 1, 1),
('Ing.', 'Bob', 'A', '0900123451', 'bob.a@example.com', 1, 1),
('Mgr.', 'Carol', 'A', '0900123452', 'carol.a@example.com', 1, 2),
('Ing.', 'David', 'A', '0900123453', 'david.a@example.com', 1, 2);

-- Company B
INSERT INTO Employees (Title, FirstName, LastName, Phone, Email, CompanyId, DepartmentId) VALUES
('Mgr.', 'Eve', 'B', '0900223450', 'eve.b@example.com', 2, 3),
('Ing.', 'Frank', 'B', '0900223451', 'frank.b@example.com', 2, 3),
('Mgr.', 'Grace', 'B', '0900223452', 'grace.b@example.com', 2, 4),
('Ing.', 'Heidi', 'B', '0900223453', 'heidi.b@example.com', 2, 4);

-- Assign Leaders
UPDATE Companies SET LeaderId = 1 WHERE Id = 1;
UPDATE Companies SET LeaderId = 5 WHERE Id = 2;

UPDATE Divisions SET LeaderId = 2 WHERE Id = 1;
UPDATE Divisions SET LeaderId = 6 WHERE Id = 3;

UPDATE Projects SET LeaderId = 3 WHERE Id = 2;
UPDATE Projects SET LeaderId = 7 WHERE Id = 4;

UPDATE Departments SET LeaderId = 4 WHERE Id = 2;
UPDATE Departments SET LeaderId = 8 WHERE Id = 4;