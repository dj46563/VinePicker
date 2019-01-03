CREATE Table Vine (
    VineId          int constraint VineId_pk primary key identity,
    Description     nvarchar(200) NOT NULL,
    VideoUrl        varchar(200) NOT NULL,
    Created         date NOT NULL,
    Permalink       varchar(100) NOT NULL,
    Loops           varchar(20) NOT NULL,
    Likes           varchar(20) NOT NULL,
    Username        nvarchar(100) NOT NULL,
    Rating          int NOT NULL,
    Submitter       varchar(100) NOT NULL,
    ThumbnailUrl    varchar(200)
);