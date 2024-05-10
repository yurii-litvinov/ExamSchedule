create table Location
(
    Location_ID int         not null GENERATED ALWAYS AS IDENTITY,
    Classroom   varchar(50) not null,
    CONSTRAINT Location_PK PRIMARY KEY (Location_ID)
);

create table Exam_Type
(
    Exam_Type_ID int          not null,
    Title        varchar(100) not null,
    CONSTRAINT Exam_Type_PK PRIMARY KEY (Exam_Type_ID)
);

create table Student
(
    Student_ID    int         not null GENERATED ALWAYS AS IDENTITY,
    First_Name    varchar(20) not null,
    Last_Name     varchar(20) not null,
    Middle_Name   varchar(20) not null,
    Student_Group varchar(10) not null,
    CONSTRAINT Student_PK PRIMARY KEY (Student_ID)
);

create table Role
(
    Role_ID int         not null,
    Title   varchar(20) not null,
    CONSTRAINT Role_PK PRIMARY KEY (Role_ID)
);

create table Staff
(
    Staff_ID             int          not null GENERATED ALWAYS AS IDENTITY,
    First_Name           varchar(20)  not null,
    Last_Name            varchar(20)  not null,
    Middle_Name          varchar(20)  not null,
    Role_ID              int          not null,
    Email                varchar(50)  not null,
    Password             varchar(256) not null,
    Refresh_Token        varchar(128),
    Refresh_Token_Expiry timestamp,
    CONSTRAINT Staff_PK PRIMARY KEY (Staff_ID),
    CONSTRAINT Role_PK FOREIGN KEY (Role_ID) REFERENCES Role (Role_ID)
);

CREATE OR REPLACE FUNCTION check_lecturer_role(user_id INT)
    RETURNS BOOLEAN AS
$$
BEGIN
    IF EXISTS (SELECT 1
               FROM Staff
               WHERE Staff_ID = user_id
                 AND Role_ID = 2) THEN
        RETURN TRUE;
    ELSE
        RETURN FALSE;
    END IF;
END;
$$ LANGUAGE plpgsql;

create table Exam
(
    Exam_ID     int          not null GENERATED ALWAYS AS IDENTITY,
    Title       varchar(100) not null,
    Type_ID     int          not null,
    Student_ID  int          not null,
    Date_Time   timestamp    not null,
    Is_Passed   bool         not null default false,
    Location_ID int          not null,
    CONSTRAINT Exam_PK PRIMARY KEY (Exam_ID),
    CONSTRAINT Type_FK FOREIGN KEY (Type_ID) REFERENCES Exam_Type (Exam_Type_ID),
    CONSTRAINT Student_FK FOREIGN KEY (Student_ID) REFERENCES Student (Student_ID),
    CONSTRAINT Location_FK FOREIGN KEY (Location_ID) REFERENCES Location (Location_ID)
);

create table Exam_Lecturer
(
    ID          int not null GENERATED ALWAYS AS IDENTITY,
    Exam_ID     int not null,
    Lecturer_ID int not null,
    CONSTRAINT Exam_FK FOREIGN KEY (Exam_ID) REFERENCES Exam (Exam_ID),
    CONSTRAINT Lecturer_FK FOREIGN KEY (Lecturer_ID) REFERENCES Staff (Staff_ID),
    CONSTRAINT CHK_LecturerRole CHECK (check_lecturer_role(Lecturer_ID))
);

insert into Role(Role_ID, Title)
VALUES (0, 'Админ');
insert into Role(Role_ID, Title)
VALUES (1, 'Сотрудник');
insert into Role(Role_ID, Title)
VALUES (2, 'Преподаватель');

insert into Location(classroom)
values ('3389');

insert into Exam_Type(Exam_Type_ID, Title)
values (0, 'Сдача');
insert into Exam_Type(Exam_Type_ID, Title)
values (1, 'Пересдача');
insert into Exam_Type(Exam_Type_ID, Title)
values (2, 'Комиссия');

insert into Student(First_Name, Last_Name, Middle_Name, Student_Group)
VALUES ('Аноним', 'Анонимов', 'Анонимович', '22.Б22');

insert into Staff(First_Name, Last_Name, Middle_Name, Role_ID, Email, Password)
values ('Лектор', 'Лекторов', 'Лекторович', 2, 'lektor@mail.ru',
        '$2a$11$Aa2mUjwbsOPVFVZiBnI8CepLV8ndaehHZgGrXmAYV.5eVUus/a9li');

insert into Exam(Title, Type_ID, Student_ID, Date_Time, Location_ID)
values ('Экзамен', 0, 1, '2022-08-30 10:10:10', 1);
insert into Exam(Title, Type_ID, Student_ID, Date_Time, Location_ID)
values ('Другой экзамен', 0, 1, '2022-10-30 10:10:10', 1);
insert into Exam(Title, Type_ID, Student_ID, Date_Time, Location_ID)
values ('Еще один экзамен', 0, 1, '2024-12-30 10:10:10', 1);

insert into Exam_Lecturer(Exam_ID, Lecturer_ID)
VALUES (1, 1);
insert into Exam_Lecturer(Exam_ID, Lecturer_ID)
VALUES (2, 1);
insert into Exam_Lecturer(Exam_ID, Lecturer_ID)
VALUES (3, 1);

insert into Staff(First_Name, Last_Name, Middle_Name, Role_ID, Email, Password)
values ('', '', '', 0, 'admin', '$2a$11$Aa2mUjwbsOPVFVZiBnI8CepLV8ndaehHZgGrXmAYV.5eVUus/a9li')