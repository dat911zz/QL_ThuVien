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
Go

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
Go

Create Trigger TG_TaoTaiKhoan on TaiKhoan
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
Go


Create Trigger TG_XoaTaiKhoan on TaiKhoan
For Delete
As
	Begin
		Declare @tenDN varchar(30), @P_XoaLogin varchar(200), @P_XoaUser varchar(200)
		Set @tenDN = (select TENDN from Deleted)
		Set @P_XoaLogin = 'Drop Login ' + @tenDN
		Set @P_XoaUser = 'Drop User ' + @tenDN

		Execute (@P_XoaLogin)
		Execute (@P_XoaUser)
	End
Go


Create Trigger TG_SuaTaiKhoan on TaiKhoan
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
		Set @P_DoiMK = 'Alter Login ' + @tenDN + ' With Password = ''' + @mKMoi + ''' Old_Password = ''' + @mK + ''''
		Set @P_SuaLogin = 'Alter Login ' + @tenDN +' With Name = ' + @tenDNMoi
		Set @P_SuaUser = 'Alter User ' + @tenDN +' With Name = ' + @tenDNMoi
		Set @P_ThemQuyenNV = 'Alter Role NhanVien Add Member ' + @tenDNMoi
		Set @P_ThemQuyenQTV = 'Alter Role QuanTriVien Add Member ' + @tenDNMoi
		Set @P_XoaQuyenNV = 'Alter Role NhanVien Drop Member ' + @tenDNMoi
		Set @P_XoaQuyenQTV = 'Alter Role QuanTriVien Drop Member ' + @tenDNMoi

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
Go

create proc SP_ThemTaiKhoan @tenDN varchar(30), @mK varchar(10), @chucVu nvarchar(30)
as
	SET XACT_ABORT ON
	begin tran
	insert into TAIKHOAN (TENDN,MATKHAU,CHUCVU)
	values (@tenDN,@mK,@chucVu)
	commit 
go


create proc SP_XoaTaiKhoan @maTK int
as 
	SET XACT_ABORT ON
	begin tran
	delete TAIKHOAN 
	where MATAIKHOAN = @maTK
	commit 
go

create proc SP_CapNhatTaiKhoan @maTK int, @tenDN varchar(30), @mK varchar(10), @chucVu nvarchar(30)
as
	SET XACT_ABORT ON
	begin tran
	update TAIKHOAN
	set TENDN = @tenDN,MATKHAU = @mK,CHUCVU = @chucVu
	where MATAIKHOAN = @maTK
	commit 
go


Create proc PC_ThemTaiKhoan @tenDN varchar(30), @mK varchar(10), @chucVu nvarchar(30)
as
begin 
	insert into TAIKHOAN (TENDN,MATKHAU,CHUCVU)
	values (@tenDN,@mK,@chucVu)
end
Go


Create Procedure ThemNhanVien (@maTK int, @hoTen nvarchar(120), @ngaySinh DateTime, @SDT char(10), @diaChi nvarchar(200), @email varchar(120))
As
	Insert Into NHANVIEN Values (@maTK, @hoTen, @ngaySinh, @SDT, @diaChi, @email)
Go


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

alter Procedure TaoTaiKhoanNhanVien 
(@hoTen nvarchar(120),
@ngaySinh DateTime, 
@SDT char(10), 
@diaChi nvarchar(200), 
@email varchar(120), 
@tenDN varchar(30), 
@mK varchar(10), 
@chucVu nvarchar(30))
As
	Begin
		Execute SP_ThemTaiKhoan @tenDN, @mK, @chucVu
		Declare @maTK int
		Set @maTK = (Select MaTaiKhoan From TaiKhoan Where TenDN = @tenDN)
		Execute ThemNhanVien @maTK, @hoTen, @ngaySinh, @SDT, @diaChi, @email
		Update TaiKhoan 
		Set MaNhanVien = (Select MaNhanVien From NhanVien Where SoDienThoai = @SDT)
		Where MaTaiKhoan = @maTK
	End
Go

exec TaoTaiKhoanNhanVien 
go
Create Procedure XoaNhanVien (@maNV int)
As
	Delete From NhanVien
	Where MaNhanVien = @maNV
Go

--Cấp quyền cho nhóm Nhân viên
grant select, insert, update, delete on SACH to NhanVien 
grant select, insert, update, delete on CHITIETMUONPHONG to NhanVien
grant select, insert on THETHUVIEN to NhanVien
grant select, insert, update, delete on PHIEUMUON to NhanVien
grant select, insert, update, delete on CHITIETMUONSACH to NhanVien
grant select, insert, update, delete on PHIEUTRA to NhanVien
grant select, insert, update, delete on CHITIETTRASACH to NhanVien
grant select, insert, update, delete on BanSaoSach to NhanVien
grant select, insert, update, delete on BiViPham to NhanVien
grant select, insert, update, delete on ChuDe to NhanVien
grant select, insert, update, delete on CungCap to NhanVien
grant select, insert, update, delete on NhaCungCap to NhanVien
grant select, insert, update, delete on NhaXuatBan to NhanVien
grant select, insert, update, delete on PhieuDatHang to NhanVien
grant select, insert, update, delete on PhieuGiao to NhanVien
grant select, insert, update, delete on Phong to NhanVien
grant select, insert, update, delete on TacGia to NhanVien
grant select, insert, update, delete on ThamGia to NhanVien
grant select, insert, update, delete on ThongBao to NhanVien
grant select, insert, update, delete on ViPham to NhanVien
grant select, insert, update on TheThuVien to NhanVien
Go

--Cấp quyền cho nhóm quản trị viên
Grant Select, Insert, Update, Delete On CungCap To QuanTriVien With Grant Option
Grant Select, Insert, Update, Delete On ChuDe To QuanTriVien With Grant Option
Grant Select, Insert, Update, Delete On BiViPham To QuanTriVien With Grant Option
Grant Select, Insert, Update, Delete On NhaCungCap To QuanTriVien With Grant Option
Grant Select, Insert, Update, Delete On NhaXuatBan To QuanTriVien With Grant Option
Grant Select, Insert, Update, Delete On PhieuDatHang To QuanTriVien With Grant Option
Grant Select, Insert, Update, Delete On PhieuGiao To QuanTriVien With Grant Option
Grant Select, Insert, Update, Delete On Phong To QuanTriVien With Grant Option
Grant Select, Insert, Update, Delete On TacGia To QuanTriVien With Grant Option
Grant Select, Insert, Update, Delete On ThamGia To QuanTriVien With Grant Option
Grant Select, Insert, Update, Delete On ThongBao To QuanTriVien With Grant Option
Grant Select, Insert, Update, Delete On ViPham To QuanTriVien With Grant Option
Grant Select, Insert, Update, Delete On BanSaoSach To QuanTriVien With Grant Option
grant select, insert, update, delete on SACH to QuanTriVien With Grant Option
grant select, insert, update, delete on NHANVIEN to QuanTriVien With Grant Option
grant select, insert, update, delete on CHITIETMUONPHONG to QuanTriVien With Grant Option
grant select, insert, update, delete on THETHUVIEN to QuanTriVien With Grant Option
grant select, insert, update, delete on PHIEUMUON to QuanTriVien With Grant Option
grant select, insert, update, delete on CHITIETMUONSACH to QuanTriVien With Grant Option
grant select, insert, update, delete on PHIEUTRA to QuanTriVien With Grant Option
grant select, insert, update, delete on CHITIETTRASACH to QuanTriVien With Grant Option
grant select, insert, update, delete on TAIKHOAN to QuanTriVien With Grant Option

Go

Create login NV1 with password = '123', Default_Database = QLThuVien
Create login NV2 with password = '123', Default_Database = QLThuVien
Create login NV3 with password = '123', Default_Database = QLThuVien
Create login NV4 with password = '123', Default_Database = QLThuVien
Create login NV5 with password = '123', Default_Database = QLThuVien

Create User NV1 For Login NV1
Create User NV2 For Login NV2
Create User NV3 For Login NV3
Create User NV4 For Login NV4
Create User NV5 For Login NV5

Alter Role QuanTriVien Add Member NV1
Alter Role NhanVien Add Member NV2
Alter Role NhanVien Add Member NV3
Alter Role NhanVien Add Member NV4
Alter Role NhanVien Add Member NV5

set dateformat dmy
go
exec dbo.TaoTaiKhoanNhanVien N'Tài khoản test', '19/02/2002', '0', '0', '0', '0s0s1', '123', N'Quản trị viên'
exec SP_CapNhatTaiKhoan 7, 'tester', '123', N'Quản trị viên' 

alter proc SP_XoaTaiKhoan @maTK int
as 
	SET XACT_ABORT ON
	begin tran
	Update NhanVien Set MaTaiKhoan = null Where MANHANVIEN = (Select MaNhanVien From TaiKhoan Where MATAIKHOAN = @maTK)
	delete TAIKHOAN 
	where MATAIKHOAN = @maTK
	commit tran
go

exec SP_CapNhatTaiKhoan 28, 'Hitler', '123', N'Nhân viên'