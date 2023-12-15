create table Location
(
    Location_ID int         not null,
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

create table Lecturer
(
    Lecturer_ID int         not null GENERATED ALWAYS AS IDENTITY,
    First_Name  varchar(20) not null,
    Last_Name   varchar(20) not null,
    Middle_Name varchar(20) not null,
    Email       varchar(50) not null,
    Checksum    varchar(50) not null,
    CONSTRAINT Lecturer_PK PRIMARY KEY (Lecturer_ID)
);

create table Employee
(
    Employee_ID int         not null GENERATED ALWAYS AS IDENTITY,
    First_Name  varchar(20) not null,
    Last_Name   varchar(20) not null,
    Middle_Name varchar(20) not null,
    Email       varchar(50) not null,
    Checksum    varchar(50) not null,
    CONSTRAINT Employee_PK PRIMARY KEY (Employee_ID)
);

create sequence exam_id_seq
    start 1
    increment 1
    NO MAXVALUE
    CACHE 1;

create table Exam
(
    Exam_ID     int          not null,
    Title       varchar(100) not null,
    Type_ID     int          not null,
    Student_ID  int          not null,
    Date_Time   timestamp    not null,
    Location_ID int          not null,
    CONSTRAINT Exam_PK PRIMARY KEY (Exam_ID),
    CONSTRAINT Type_FK FOREIGN KEY (Type_ID) REFERENCES Exam_Type (Exam_Type_ID),
    CONSTRAINT Student_FK FOREIGN KEY (Student_ID) REFERENCES Student (Student_ID),
    CONSTRAINT Location_FK FOREIGN KEY (Location_ID) REFERENCES Location (Location_ID)
);

create table Exam_Lecturer
(
    Exam_ID     int not null,
    Lecturer_ID int not null,
    CONSTRAINT Exam_FK FOREIGN KEY (Exam_ID) REFERENCES Exam (Exam_ID),
    CONSTRAINT Lecturer_FK FOREIGN KEY (Lecturer_ID) REFERENCES Lecturer (Lecturer_ID)
);

insert into Location(location_id, classroom)
values (0, '3389');

insert into Exam_Type(Exam_Type_ID, Title)
values (0, 'Пересдача');

insert into Student(First_Name, Last_Name, Middle_Name, Student_Group)
VALUES ('Аноним', 'Анонимов', 'Анонимович', '22.Б22');

insert into Lecturer(First_Name, Last_Name, Middle_Name, Email, Checksum)
values ('Лектор', 'Лекторов', 'Лекторович', 'lektor@mail.ru', 'lektor');

insert into Exam(Exam_ID, Title, Type_ID, Student_ID, Date_Time, Location_ID)
values (nextval('exam_id_seq'), 'Экзамен', 0, 1, '2022-08-30 10:10:10', 0);
insert into Exam(Exam_ID, Title, Type_ID, Student_ID, Date_Time, Location_ID)
values (nextval('exam_id_seq'), 'Другой экзамен', 0, 1, '2022-10-30 10:10:10', 0);
insert into Exam(Exam_ID, Title, Type_ID, Student_ID, Date_Time, Location_ID)
values (nextval('exam_id_seq'), 'Еще один экзамен', 0, 1, '2024-12-30 10:10:10', 0);


insert into Exam_Lecturer(Exam_ID, Lecturer_ID)
VALUES (1, 1);
insert into Exam_Lecturer(Exam_ID, Lecturer_ID)
VALUES (2, 1);
insert into Exam_Lecturer(Exam_ID, Lecturer_ID)
VALUES (3, 1);

-- drop table Exam_Lecturer;
-- drop table Exam;
-- drop sequence exam_id_seq;
-- drop table Employee;
-- drop table Lecturer;
-- drop table Student;
-- drop table exam_type;
-- drop table Location;