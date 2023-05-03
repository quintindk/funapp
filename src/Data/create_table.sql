CREATE TABLE Blogs(
   BlogId INT PRIMARY KEY NOT NULL,
   [Name] TEXT
);

CREATE TABLE Posts(
   PostId INT PRIMARY KEY NOT NULL,
   [Name] TEXT,
   [Title] TEXT,
   [Content] TEXT,
   BlogId INT
);