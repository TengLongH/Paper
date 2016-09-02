create table Author_Person(
	Id int identity(1,1),
	AuthorId int ,
	PersonNo varchar(64),

	/*若名字为外单位人员，则说明为外单位*/

	Name varchar(127),

	/*
	0: 未确认
	1：确认正确
	2：确认错误
	3：已认领
	4：已驳回
	*/
	
	status int default(0),
	constraint Author_Person_PK primary key (Id),
	constraint AP_Author foreign key (AuthorId) references Author(Id)
);