USE QLTHUVIEN
GO

--thống kê phòng theo nam
create function fn_ThongKeSLPhong(@namTK datetime)
returns varchar(120)
as 
	begin
	declare @kq varchar(MAX), @thang int,@soPhong int
		set @thang = 1 
		set @kq = ' '
		while (@thang < 13)
		begin 		
			set @soPhong = (
						select  count(year(THOIGIANMUON))
						from PHONG p,CHITIETMUONPHONG ct,THETHUVIEN t, NGUOISUDUNG n 
						where p.MAPHONG = ct.MAPHONG and t.MANGUOISUDUNG = ct.MANGUOISUDUNG and t.MANGUOISUDUNG = n.MANGUOISUDUNG and   MONTH(ct.THOIGIANMUON) = @thang and DATEDIFF(YEAR,ct.THOIGIANMUON,'2022-03-06') =0
						group by year(ct.THOIGIANMUON)
						)
			set @kq = @kq + convert(varchar(50),@soPhong)+','
			set @thang = @thang +1
		end
		return @kq
	end
go

drop function fn_ThongKeSLPhong

select dbo.fn_ThongKeSLPhong('2022-03-06')
--thời gian mượn phòng < thời gian trả phòng 


alter trigger TRG_TGMuon on CHITIETMUONPHONG
for insert
as
begin
	declare @maP int , @tgm datetime,@tgt datetime, @kq int
	set @maP = (select MAPHONG from inserted)
	set @tgm = (select THOIGIANMUON from inserted)
	set @tgt = (select THOIGIANTRA from inserted)
	set @kq = (select dbo.FN_KtraThoiGian(@maP,@tgm,@tgt))
	if(@kq = 0)
		commit 
	else 
		rollback 

end
go

insert into CHITIETMUONPHONG
values (1,1,3,N'2023-12-30 13:13:00','2023-12-30 16:15:00')

go
create function FN_KtraThoiGian (@maP int, @m datetime, @t datetime)
returns int
as
begin
	declare @kq int
	set @kq = 0
	declare CR_TG cursor
	for	select THOIGIANMUON, THOIGIANTRA  from CHITIETMUONPHONG where MAPHONG =@maP
	declare @tgm datetime,@tgt datetime
	open CR_TG

	fetch next from CR_TG into @tgm,@tgt
	while(@@FETCH_STATUS = 0)
	begin	
		if(DATEDIFF(DAY,@m,@tgt) > 0)
		begin
			set @kq =1
		end
		else if(DATEDIFF(DAY,@m,@tgt) = 0 and DATEDIFF(MINUTE,@m,@tgt) > 0)
		begin
			set @kq =1
		end
		fetch next from CR_TG into @tgm,@tgt
	end
	close CR_TG
	deallocate CR_TG
	return @kq
end
go

declare @a int
set @a = (select dbo.FN_TG(1,'2023-01-02 13:13:00.000','2023-01-02 11:00:00.000'))
if(@a = 0)
print ('abc')
else
print ('cde')
select DATEDIFF(MINUTE, '2023-1-2 10:13:00.000','2023-1-2 11:00:00.000')
select DATEDIFF(DAY, '2022-12-30 13:13:00.000','2022-12-31 16:15:00.000')
select THOIGIANMUON, THOIGIANTRA  from CHITIETMUONPHONG where MAPHONG =1
go
	declare @kq int
	set @kq = 0
	declare CR_TG cursor
	for	select THOIGIANMUON, THOIGIANTRA  from CHITIETMUONPHONG where MAPHONG =1
	declare @tgm datetime,@tgt datetime
	open CR_TG

	fetch next from CR_TG into @tgm,@tgt
	while(@@FETCH_STATUS = 0)
	begin	
		if(DATEDIFF(DAY,'2023-01-03 08:10:00.000',@tgt) > 0)
		begin
			print(@tgt)
			set @kq =1
		end
		if(DATEDIFF(DAY,'2023-01-03 08:10:00.000',@tgt) = 0 and DATEDIFF(MINUTE,'2023-01-03 08:10:00.000',@tgt) > 0)
		begin
			print('1')
			print(@tgt)
			set @kq =1
		end
		fetch next from CR_TG into @tgm,@tgt
	end
	close CR_TG
	deallocate CR_TG
	print(@kq)
--hàm thống kê phòng chờ xác nhận 
create function FN_PhongChoXN()
returns table
as
	return 
	select * from CHITIETMUONPHONG where MANHANVIEN is null

select * from dbo.FN_PhongChoXN()


--thêm người sử dụng 
create proc PC_ADDNSD @hoten nvarchar(120), @NS date,@SDT char(10),@NN bit,@email varchar(120),@diaChi nvarchar(256)
as
begin
	insert into NGUOISUDUNG
	values (@hoten,@NS,@sdt,@NN,@email,@diaChi)
end

exec PC_ADDNSD N'Nguyễn Văn Tèo','1996-04-29','0564789123',1,'',N'109/52 Nguyễn Thiện Thuật, Phường 2, Quận 3'

--xóa người sử dụng
create proc PC_DeleteNSD @maNSD int
as
begin
	declare @maNSD int 
	set @maNSD = (select MANGUOISUDUNG from deleted)
	if(exists(select MANGUOISUDUNG from PHIEUMUON where MANGUOISUDUNG = @maNSD))
	begin 
		delete PHIEUMUON
		where MANGUOISUDUNG = @maNSD
	end
	if(exists(select * from CHITIETMUONPHONG where MANGUOISUDUNG = @maNSD))
	begin 
		delete CHITIETMUONPHONG
		where MANGUOISUDUNG = @maNSD
	end
	if(exists(select MANGUOISUDUNG from THETHUVIEN where MANGUOISUDUNG = @maNSD)) 
	begin 
		delete THETHUVIEN
		where MANGUOISUDUNG = @maNSD
	end
	if(exists(select MANGUOISUDUNG from NGUOISUDUNG where MANGUOISUDUNG = @maNSD)) 
	begin 
		delete NGUOISUDUNG
		where MANGUOISUDUNG = @maNSD
	end
end

exec PC_DeleteNSD 47

--cap nhaap nguoi su dung
create proc PC_UPDATENSD @ma int, @hoten nvarchar(120), @NS date,@SDT char(10),@NN bit,@email varchar(120),@diaChi nvarchar(256)
as
begin
	update NGUOISUDUNG
	set HOTEN = @hoten,NGAYSINH= @NS,SODIENTHOAI = @sdt,NGUOINGOAI = @NN,EMAIL = @email,DIACHI = @diaChi
	where MANGUOISUDUNG = @ma
end


exec PC_UPDATENSD 47,N'Nguyen','1996-04-29','0564789123',1,'',N'109/52 Nguyễn Thiện Thuật, Phường 2, Quận 3'


--Constraint 

Alter Table TheThuVien
Add Constraint Check_NgayCap Check (NgayHetHan <= GetDate())

Alter Table TheThuVien
Add Constraint Check_NgayHetHan Check (NgayHetHan >= NgayCap)

Alter Table ThongBao
Add Constraint Check_NgayThongBao Check (ThoiGian <= Getdate())
Go

--Function
Create Function F_TimPhongDuocMuonTrongKhoangThoiGianAB (@ThoiGianA DateTime, @ThoiGianB DateTime)
Returns Table
As
	Return (Select * From ChiTietMuonPhong
			Where ThoiGianMuon >= @ThoiGianA And ThoiGianTra <= @ThoiGianB)
Go
select * from F_TimPhongDuocMuonTrongKhoangThoiGianAB ('2022-12-30 8:13:00','2022-12-30 16:15:00')


Create Function F_TimPhongTrongTrongKhoangThoiGianAB (@ThoiGianA DateTime, @ThoiGianB DateTime)
Returns Table
As
	Return (Select Distinct MaPhong From ChiTietMuonPhong
			Where MaPhong Not In (Select MaPhong From CHITIETMUONPHONG
								Where ThoiGianMuon >= @ThoiGianA And ThoiGianTra <= @ThoiGianB ))
Go
select * from F_TimPhongTrongTrongKhoangThoiGianAB ('2022-12-30 8:13:00','2022-12-30 16:15:00')

Create Function F_TimSach(@ChuoiTimKiem nvarchar (200))
Returns Table
As
	Return (Select Distinct * From Sach Where 
			MaSach = @ChuoiTimKiem Or
			MaNXB = @ChuoiTimKiem Or
			MaChuBien = @ChuoiTimKiem Or
			MaChuDe = @ChuoiTimKiem Or
			TenSach Like @ChuoiTimKiem Or
			MaNXB = @ChuoiTimKiem)
--Procedure

Create Procedure ThemNhanVien (@maTK int, @hoTen nvarchar(120), @ngaySinh DateTime, @SDT char(10), @diaChi nvarchar(200), @email varchar(120))
As
	Insert Into NHANVIEN Values (@maTK, @hoTen, @ngaySinh, @SDT, @diaChi, @email)
Go
exec ThemNhanVien 1,N'Nguyen Bao Thien','1999-11-24','0886324512','',''

Create Procedure XoaNhanVien (@maNV int)
As
	Delete From NhanVien
	Where MaNhanVien = @maNV
Go
exec XoaNhanVien 7

Create Procedure SuaNhanVien (@maNV int, @maTK int, @hoTen nvarchar(120), @ngaySinh DateTime, @SDT char(10), @diaChi nvarchar(200), @email varchar(120))
As
	UPDATE NHANVIEN
    SET    MaTaiKhoan = @maTK,
            HoTen = @hoTen,
            NgaySinh = @ngaySinh,
            DiaChi = @diaChi,
			Email = @email
    WHERE  MaNhanVien = @maNV
Go

--Trigger

alter Trigger CapNhapTinhTrangSachMuon On ChiTietMuonSach
After Insert
As
	Begin
		Declare @maS int, @maBS int
		
		Declare Cs_LayMaSach Cursor Local for
			Select Distinct MaSach From Inserted

		Open Cs_LayMaSach

		While (1=1)
			Begin
				Fetch Next From Cs_LayMaSach Into @maS
				If(@@FETCH_STATUS = 0)
					Begin
						Declare Cs_LayMaBanSao Cursor Local for
							Select Distinct MaBanSao From Inserted
							Where MaSach = @maS

						Open Cs_LayMaBanSao

						While (1=1)
							Begin
								Fetch Next From Cs_LayMaBanSao Into @maBS
								If(@@FETCH_STATUS = 0)
									Begin
										Update BanSaoSach
										Set TinhTrang = 0
										Where MaSach = @maS And MaBanSao = @maBS
									End
								Else
									Break
							End
						Close Cs_LayMaBanSao
						Deallocate Cs_LayMaBanSao
					End
				Else
					Break
			End
		Close Cs_LayMaSach
		Deallocate Cs_LayMaSach
	End
Go


Create Trigger CapNhapTinhTrangSachTra On ChiTietTraSach
After Insert
As
	Begin
		Declare @maS int, @maBS int
		
		Declare Cs_LayMaSach Cursor Local for
			Select Distinct MaSach From Inserted

		Open Cs_LayMaSach

		While (1=1)
			Begin
				Fetch Next From Cs_LayMaSach Into @maS
				If(@@FETCH_STATUS = 0)
					Begin
						Declare Cs_LayMaBanSao Cursor Local for
							Select Distinct MaBanSao From Inserted
							Where MaSach = @maS

						Open Cs_LayMaBanSao

						While (1=1)
							Begin
								Fetch Next From Cs_LayMaBanSao Into @maBS
								If(@@FETCH_STATUS = 0)
									Begin
										Update BanSaoSach
										Set TinhTrang = 1
										Where MaSach = @maS And MaBanSao = @maBS
									End
								Else
									Break
							End
						Close Cs_LayMaBanSao
						Deallocate Cs_LayMaBanSao
					End
				Else
					Break
			End
		Close Cs_LayMaSach
		Deallocate Cs_LayMaSach
	End
Go

--HE


create proc ThemSACH		
		@manxb int,
		@machude int, 
		@tensach nvarchar(120),
		@mota nvarchar(max),
		@namxuatban datetime,
		@giasach int,
		@anhbia char(120)		
as 
begin
	insert into SACH(MANXB,MACHUDE,TENSACH,MOTA,NAMXUATBAN,GIASACH,ANHBIA)
	values (@manxb,@machude,@tensach,@mota,@namxuatban,@giasach,@anhbia)
end
go
exec ThemSACH 1,1,N'yadru',	'mot cuoc doi','2021-12-7',60000,''


create proc XoaSach
		@masach int
as begin
	delete from SACH
	where MASACH =@masach
end
go
exec XoaSach 1

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
go
alter table sach
add constraint CK_NXB check (NAMXUATBAN < getdate() )


--hieu

-- hàm thêm chi tiết mượn phòng 
create proc ADD_ChiTietMuonPhong @maNSD int, @maphong int, @manv int, @TGM date, @TGT date 
as 
begin 
	insert into CHITIETMUONPHONG
	values (@maNSD,@maphong,@manv,@TGM,@TGT)
end 

exec ADD_ChiTietMuonPhong '5', '1', N'2022-06-12T15:16:00.000',N'2022-06-12T16:15:00.000'

--cập nhật chi tiết mượn phòng 
go
create proc UpdateChiTietMuonPhong @maNSD int , @maphong int, @manv int, @TGM date, @TGT date 
as 
begin
	update CHITIETMUONPHONG 
	set MAPHONG = @maphong, MANHANVIEN = @manv, THOIGIANMUON = @TGM, THOIGIANTRA = @TGT
	where MANGUOISUDUNG = @maNSD
end

exec UpdateChiTietMuonPhong '5', '1', N'2022-06-12T15:16:00.000',N'2022-06-12T16:15:00.000'

go
create proc DEL_ChiTietMuonPhong @maNSD int 
as 
begin 
	delete CHITIETMUONPHONG
	where MANGUOISUDUNG = @maNSD
end






go
use master
go
---Tạo nhóm quyền
create role NhanVien
go
create role QuanTriVien
go
--Cấp quyền cho nhóm Nhân viên
grant select, insert, update, delete on SACH to NhanVien
grant select, insert, update, delete on CHITIETMUONPHONG to NhanVien
grant select, insert on THETHUVIEN to NhanVien
grant select, insert, update, delete on PHIEUMUON to NhanVien
grant select, insert, update, delete on CHITIETMUONSACH to NhanVien
grant select, insert, update, delete on PHIEUTRA to NhanVien
grant select, insert, update, delete on CHITIETTRASACH to NhanVien

--Cấp quyền cho nhóm quản trị viên
grant select, insert, update, delete on SACH to QuanTriVien
grant select, insert, update, delete on NHANVIEN to QuanTriVien
grant select, insert, update, delete on CHITIETMUONPHONG to QuanTriVien
grant select, insert, update, delete on THETHUVIEN to QuanTriVien
grant select, insert, update, delete on PHIEUMUON to QuanTriVien
grant select, insert, update, delete on CHITIETMUONSACH to QuanTriVien
grant select, insert, update, delete on PHIEUTRA to QuanTriVien
grant select, insert, update, delete on CHITIETTRASACH to QuanTriVien
grant select, insert, update, delete on TAIKHOAN to QuanTriVien



select * from TAIKHOAN
Insert into Taikhoan Values ('123',N'Quản Trị Viên','test2')
Alter login test2 With name = test

create Trigger TG_TaoTaiKhoan on TaiKhoan
For Insert 
As
Begin
	Declare @tenDN varchar(30), @mK varchar(10), @chucVu nvarchar(30), @P_TaoLogin varchar(200), 
		@P_TaoUser varchar(200), @P_ThemQuyenNV varchar(200), @P_ThemQuyenQTV varchar(200)
	Set @tenDN = (select TENDN from inserted)
	Set @mK = (select MATKHAU from inserted)
	Set @chucVu = (select CHUCVU from inserted)
	Set @P_TaoLogin = 'Create Login ' + @tenDN +' With Password = ''' + @mK + ''', Default_Database = QLThuVien'
	Set @P_TaoUser = 'Create User ' + @tenDN +' For Login ' + @tenDN
	Set @P_ThemQuyenNV = 'Alter Role NhanVien Add Member ' + @tenDN
	Set @P_ThemQuyenQTV = 'Alter Role QuanTriVien Add Member ' + @tenDN

	Execute (@P_TaoLogin)
	Execute (@P_TaoUser)

	if(@chucVu = N'Nhân viên')
		Execute (@P_ThemQuyenNV)
	else if(@chucVu = N'Quản trị viên')
		Execute (@P_ThemQuyenQTV)
End
create login NV5  with password = '123456', default_database = QLTHUVIEN1
create user NV5 for login NV5
alter role NhanVien add Member NV5
create user nvy for login nvy

go
	create login ten with password = '2', default_database = QLTHUVIEN;



select * from taikhoan
go
create Trigger TG_XoaTaiKhoan on TaiKhoan
For Delete
As
	Begin
		Declare @tenDN varchar(30), @P_XoaLogin varchar(200), @P_XoaUser varchar(200)
		Set @tenDN = (select TENDN from inserted)
		Set @P_XoaLogin = 'Drop Login ' + @tenDN
		Set @P_XoaUser = 'Drop User ' + @tenDN

		Execute (@P_XoaLogin)
		Execute (@P_XoaUser)
	End
Go

create Trigger TG_SuaTaiKhoan on TaiKhoan
For Update 
As
	Begin
		Declare @tenDN varchar(30), @mK varchar(10), @chucVu nvarchar(30), @tenDNMoi varchar(30), @mKMoi varchar(10) ,
			@P_SuaLogin varchar(200), @P_SuaUser varchar(200), @P_ThemQuyenNV varchar(200), @P_DoiMK varchar(200),
			@P_ThemQuyenQTV varchar(200), @P_XoaQuyenNV varchar(200), @P_XoaQuyenQTV varchar (200)
		Set @tenDN = (select TENDN from deleted)
		Set @mK = (select MATKHAU from deleted)
		Set @tenDNMoi = (select TENDN from inserted)
		Set @mKMoi = (select MATKHAU from inserted)
		Set @chucVu = (select CHUCVU from inserted)
		Set @P_DoiMK = 'Alter Login ' + @tenDN + ' With Password = ''' + @mKMoi + ''' Old_Password = ' + @mK
		Set @P_SuaLogin = 'Alter Login ' + @tenDN +' With Name = ' + @tenDNMoi
		Set @P_SuaUser = 'Alter User ' + @tenDN +' With Name = ' + @tenDNMoi
		Set @P_ThemQuyenNV = 'Alter Role NhanVien Add Member ' + @tenDN
		Set @P_ThemQuyenQTV = 'Alter Role QuanTriVien Add Member ' + @tenDN
		Set @P_XoaQuyenNV = 'Alter Role NhanVien Drop Member ' + @tenDN
		Set @P_XoaQuyenQTV = 'Alter Role QuanTriVien Drop Member ' + @tenDN

		Execute (@P_DoiMK)
		Execute (@P_SuaLogin)
		Execute (@P_SuaUser)

		if(@chucVu = N'Nhân viên')
			Begin
				Execute (@P_XoaQuyenQTV)
				Execute (@P_ThemQuyenNV)
			End
		else if(@chucVu = N'Quản trị viên')
			Begin
				Execute (@P_XoaQuyenNV)
				Execute (@P_ThemQuyenQTV)
			End
	End


create proc SP_ThemTaiKhoan @tenDN varchar(30), @mK varchar(10), @chucVu nvarchar(30)
as
	SET XACT_ABORT ON
	begin tran
	insert into TAIKHOAN (TENDN,MATKHAU,CHUCVU)
	values (@tenDN,@mK,@chucVu)
	commit 
go
exec SP_ThemTaiKhoan'vy','123',N'Nhân viên'
go
create proc SP_XoaTaiKhoan @maTK int
as 
	SET XACT_ABORT ON
	begin tran
	delete TAIKHOAN 
	where MATAIKHOAN = @maTK
	commit 
go

exec SP_XoaTaiKhoan 8

create proc SP_CapNhatTaiKhoan @maTK int, @tenDN varchar(30), @mK varchar(10), @chucVu nvarchar(30)
as
	SET XACT_ABORT ON
	begin tran
	update TAIKHOAN
	set TENDN = @tenDN,MATKHAU = @mK,CHUCVU = @chucVu
	where MATAIKHOAN = @maTK
	commit 
go

exec SP_CapNhatTaiKhoan 8 , 'ngoc','12',N'Nhân viên'

select * from Nhanvien
select * from thethuvien
select * from taikhoan

alter table  NHANVIEN
add unique(SODIENTHOAI)

alter table  NHANVIEN
add unique(EMAIL)

alter table  THETHUVIEN
add unique(SODIENTHOAI)

alter table  THETHUVIEN
add unique(EMAIL)

alter table TAIKHOAN
add unique (TENDN)


--the thu vien
create proc Them_TheThuVien @hoTen nvarchar(120),@SDT char(10),@nguoiNgoai bit,@email varchar(60),@diaChi nvarchar(200),@ngayCap date, @ngayHetHan date
as 
begin tran
	set transaction isolation
	level read uncommitted
	insert into THETHUVIEN
	values (@hoTen,@SDT,@nguoiNgoai,@email,@diaChi,@ngayCap,@ngayHetHan )
commit tran
go

--cập nhật 
go
create proc CapNhat_TheThuVien @maTTV int , @hoTen nvarchar(120),@SDT char(10),@nguoiNgoai bit,@email varchar(60),@diaChi nvarchar(200),@ngayCap date, @ngayHetHan date
as 
begin
	update  THETHUVIEN
	set HOTEN = @hoTen,SODIENTHOAI = @SDT,NGUOINGOAI = @nguoiNgoai,EMAIL = @email,DIACHI = @diaChi,NGAYCAP = @ngayCap,NGAYHETHAN =@ngayHetHan 
	where MATTV = @maTTV
end

go
create proc Xoa_TheThuVien @maNSD int 
as 
begin tran
	set transaction isolation
	level read uncommitted
	--xoa vi pham
	while(exists(select * from PHIEUTRA pt,PHIEUMUON pm,BIVIPHAM vp where  pm.MANSD = @maNSD and pt.MAPHIEUMUON = pm.MAPHIEUMUON and pt.MAPHIEUTRA = vp.MAPHIEUTRA))
	begin 	
		delete BIVIPHAM
		where MAPHIEUMUON = (select distinct top(1) vp.MAPHIEUMUON from PHIEUTRA pt,PHIEUMUON pm,BIVIPHAM vp where  pm.MANSD = @maNSD and pt.MAPHIEUMUON = pm.MAPHIEUMUON and pt.MAPHIEUTRA = vp.MAPHIEUTRA)
	end
	--xoa phieu tra
	while(exists(select * from PHIEUTRA pt,PHIEUMUON pm, CHITIETTRASACH ctts where pm.MANSD = @maNSD and pm.MAPHIEUMUON = pt.MAPHIEUMUON and pt.MAPHIEUTRA = ctts.MAPHIEUTRA))
	begin 	
		delete CHITIETTRASACH
		where MAPHIEUMUON = (select distinct top(1) ctts.MAPHIEUMUON  from CHITIETTRASACH ctts,PHIEUTRA pt,PHIEUMUON pm where ctts.MAPHIEUMUON = pt.MAPHIEUMUON and pt.MAPHIEUMUON = pm.MAPHIEUMUON and pm.MANSD = @maNSD)
	end
	while(exists(select * from PHIEUMUON pm, PHIEUTRA pt where  pm.MANSD = @maNSD and pm.MAPHIEUMUON = pt.MAPHIEUMUON))
	begin 	
		delete PHIEUTRA
		where MAPHIEUMUON = (select distinct top(1) pt.MAPHIEUMUON from PHIEUMUON pm, PHIEUTRA pt where  pm.MANSD = @maNSD and pm.MAPHIEUMUON = pt.MAPHIEUMUON)
	end
	--xoa phieu muon
	while(exists(select * from CHITIETMUONSACH ctms,PHIEUMUON pm where  pm.MANSD = @maNSD and ctms.MAPHIEUMUON = pm.MAPHIEUMUON))
	begin 	
		delete CHITIETMUONSACH
		where MAPHIEUMUON = (select distinct top(1) ctms.MAPHIEUMUON from CHITIETMUONSACH ctms,PHIEUMUON pm where  pm.MANSD = @maNSD and ctms.MAPHIEUMUON = pm.MAPHIEUMUON)
	end
	while(exists(select distinct MANSD from PHIEUMUON where MANSD = @maNSD))
	begin 
		delete PHIEUMUON
		where MANSD = @maNSD
	end
	--xoa muon phong
	while(exists(select distinct MANSD from CHITIETMUONPHONG where MANSD = @maNSD))
	begin 
		delete CHITIETMUONPHONG
		where MANSD = @maNSD
	end
	while(exists(select MATTV from THETHUVIEN where MATTV = @maNSD)) 
	begin 
		delete THETHUVIEN
		where MATTV = @maNSD
	end
commit tran


create function FN_DanhSachTTV ()
returns table
as
	return 
	select * from THETHUVIEN

drop function FN_DanhSachTTV
select * from FN_DanhSachTTV ()

--sao luu
create proc FullBackUp 
as
	backup database QLTHUVIEN1
	to disk = 'D:\DALM\QLTV_Full.bak'
	with init
go
exec FullBackUp
go
create proc DiffBackUp
as
	backup database QLTHUVIEN1
	to disk = 'D:\DALM\QLTV_Diff.bak'
	with init, differential
go
exec DiffBackUp
go
create proc LogBackUp
as
	backup log QLTHUVIEN1	
	to disk = 'D:\DALM\QLTV_Log.trn'
go
exec LogBackUp
go
create proc RestoreDatabase
as 
	restore database QLTHUVIEN
	from disk = 'D:\DALM\QLTV_Full.bak'
	with norecovery, REPLACE

	restore database QLTHUVIEN
	from disk = 'D:\DALM\QLTV_Diff.bak'
	with norecovery, REPLACE

	restore database QLTHUVIEN
	from disk = 'D:\DALM\QLTV_Log.trn'
	with recovery, REPLACE
go

--sao luu tu dong
--full
USE [msdb]
GO
EXEC msdb.dbo.sp_update_job @job_id=N'e56971bd-5c7b-45e8-be57-9179c56b5e5d', 
		@notify_level_email=2, 
		@notify_level_page=2
GO
USE [msdb]
GO
EXEC msdb.dbo.sp_update_jobstep @job_id=N'e56971bd-5c7b-45e8-be57-9179c56b5e5d', @step_id=1 , 
		@command=N'backup database QLTHUVIEN
to disk = ''D:\DALM\QLTV_Full.bak'''
GO
EXEC msdb.dbo.sp_attach_schedule @job_id=N'e56971bd-5c7b-45e8-be57-9179c56b5e5d',@schedule_id=16
GO
USE [msdb]
GO
EXEC msdb.dbo.sp_update_schedule @schedule_id=16, 
		@active_start_date=20230112
GO
--diff
USE [msdb]
GO
EXEC msdb.dbo.sp_update_jobstep @job_id=N'b437dada-e1f5-43c8-8949-38af14103191', @step_id=1 , 
		@command=N'backup database QLTHUVIEN
to disk = ''D:\DALM\QLTV_Diff.bak''
with differential
'
GO
EXEC msdb.dbo.sp_attach_schedule @job_id=N'b437dada-e1f5-43c8-8949-38af14103191',@schedule_id=17
GO
--log
USE [msdb]
GO
EXEC msdb.dbo.sp_update_jobstep @job_id=N'07d8920e-6fbb-491a-bed5-52d752daaa8b', @step_id=1 , 
		@command=N'backup log QLTHUVIEN
to disk = ''D:\DALM\QLTV_Log.trn'''
GO
EXEC msdb.dbo.sp_attach_schedule @job_id=N'07d8920e-6fbb-491a-bed5-52d752daaa8b',@schedule_id=18
GO
USE [msdb]
GO
EXEC msdb.dbo.sp_update_schedule @schedule_id=18, 
		@freq_type=4, 
		@freq_interval=1, 
		@freq_subday_type=8, 
		@freq_subday_interval=1, 
		@active_start_time=190000
GO
