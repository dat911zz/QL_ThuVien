create proc ThemSACH		
		@manxb int,
		@machude int, 
		@tensach nvarchar(120),
		@mota nvarchar(max),
		@namxuatban datetime,
		@giasach int,
		@anhbia char(120)		
as begin
insert into SACH(MANXB,MACHUDE,TENSACH,MOTA,NAMXUATBAN,GIASACH,ANHBIA)
values (@manxb,@machude,@tensach,@mota,@namxuatban,@giasach,@anhbia)
end
go



create proc XoaSach
		@masach int
as begin
delete from SACH
where MASACH =@masach
end
go

create proc SuaSach
		@manxb int,
		@machude int, 
		@tensach nvarchar(120),
		@mota nvarchar(max),
		@namxuatban datetime,
		@giasach int,
		@anhbia char(120),	
		@masach int
as begin
update SACH
set MANXB = @manxb, MACHUDE= @machude, TENSACH =@tensach, MOTA = @mota, NAMXUATBAN = @namxuatban, GIASACH=@giasach, ANHBIA =@anhbia
where MASACH = @masach
end
go


create function TraTheoCD(@machude int)
returns table 
return
	select s.MACHUDE, cd.TENCHUDE, s.TENSACH ,s.NAMXUATBAN 
	from SACH s  , CHUDE cd  
	where		s.MACHUDE = @machude
			and s.MACHUDE = cd.MACHUDE

go

drop function TraTheoCD

select * from TraTheoCD(2)

go
create function TraTheoNXB(@manxb int)
returns table return
	select s.MANXB, nxb.TENNXB , s.TENSACH
	from SACH s  , NHAXUATBAN nxb
	where s.MANXB = @manxb 
		and s.MANXB = nxb.MANXB
go

drop function TraTheoNXB

select * from TraTheoNXB(2)


select * from CHUDE
select * from SACH 

go


alter table sach
add constraint CK_gia Check (GIASACH >0)

alter table sach
add constraint CK_NXB check (NAMXUATBAN < getdate() )

