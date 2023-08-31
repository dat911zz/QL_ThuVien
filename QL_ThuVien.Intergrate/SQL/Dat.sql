USE [QLTHUVIEN]
GO
CREATE FUNCTION [dbo].[LAY_TEN_ND](@MATV INT)
RETURNS NVARCHAR(20)
AS
BEGIN
	RETURN( SELECT HOTEN FROM THETHUVIEN
			WHERE MATTV = @MATV
			)
END
GO
CREATE FUNCTION [dbo].[LAY_TEN_NV](@MANV INT)
RETURNS NVARCHAR(20)
AS
BEGIN
	RETURN( SELECT HOTEN FROM NHANVIEN
			WHERE MANHANVIEN = @MANV
			)
END
GO
CREATE FUNCTION [dbo].[LAY_TEN_PHONG](@MAPH INT)
RETURNS NVARCHAR(20)
AS
BEGIN
	RETURN( SELECT TENPHONG FROM PHONG
			WHERE MAPHONG = @MAPH
			)
END
GO
create function FN_MuonSachXN()
returns table
as
	return 
	select * from PHIEUMUON where MANHANVIEN is null
go
select * from dbo.FN_MuonSachXN()

insert into PHIEUMUON(MANSD,NGAYMUON,NGAYTRA) values(4, '2023-01-01', '2023-01-02')
go
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

grant select, insert, update, delete on dbo.FN_PhongChoXN to QuanTriVien With Grant Option
grant select, insert, update, delete on dbo.FN_MuonSachXN to QuanTriVien With Grant Option
grant select, insert, update, delete on dbo.F_TimSach to QuanTriVien With Grant Option
grant execute on LAY_TEN_ND to QuanTriVien With Grant Option
grant execute on LAY_TEN_NV to QuanTriVien With Grant Option
grant execute on LAY_TEN_PHONG to QuanTriVien With Grant Option
grant execute on TaoTaiKhoanNhanVien to QuanTriVien With Grant Option
grant execute on SP_ThemTaiKhoan to QuanTriVien With Grant Option
grant execute on ThemNhanVien to QuanTriVien With Grant Option

go

Create Function ThemCotKhongDau (@Text Nvarchar(100))
Returns Nvarchar(100)
As
Begin
	Set @Text = Lower(@Text)
	Declare @Textlen Int = Len(@Text)
	If @Textlen > 0
	Begin
		Declare @Index Int = 1
		Declare @Lpos Int
		Declare @Sign_Chars Nvarchar(100) = N'ăâđêôơưàảãạáằẳẵặắầẩẫậấèẻẽẹéềểễệếìỉĩịíòỏõọóồổỗộốờởỡợớùủũụúừửữựứỳỷỹỵýđð'
		Declare @Unsign_Chars Varchar(100) = 'aadeoouaaaaaaaaaaaaaaaeeeeeeeeeeiiiiiooooooooooooooouuuuuuuuuuyyyyydd'

		While @Index <= @Textlen
		Begin
			Set @Lpos = Charindex(Substring(@Text,@Index,1),@Sign_Chars)
			If @Lpos > 0
			Begin
				Set @Text = Stuff(@Text,@Index,1,Substring(@Unsign_Chars,@Lpos,1))
			End
			Set @Index = @Index + 1
		End
	End
	Return @Text
End
Go

Create View SachKhongDau 
As
	Select Sach.MaSach, dbo.ThemCotKhongDau(TenSach) As 'TenSach', dbo.ThemCotKhongDau(TenChuDe) As 'TenChuDe', dbo.ThemCotKhongDau(TenNXB) As 'TenNXB'
	From Sach, ChuDe, NhaXuatBan
	Where Sach.MaChuDe = ChuDe.MaChuDe And Sach.MaNXB = NhaXuatBan.MaNXB
Go

create Function F_TimSach(@ChuoiTimKiem nvarchar (200))
Returns Table
As
	Return (Select s.MASACH, s.MANXB, s.MACHUDE, s.TENSACH, MOTA, NAMXUATBAN, GIASACH, ANHBIA 
			From Sach s
			Join CHUDE cd on s.MACHUDE = cd.MACHUDE
			Join NHAXUATBAN nxb on s.MANXB = nxb.MANXB
			Join SachKhongDau skd On skd.MASACH = s.MASACH
			Where CONVERT(char , s.MASACH) Like @ChuoiTimKiem Or 
				s.TenSach Like @ChuoiTimKiem Or
				cd.TENCHUDE Like @ChuoiTimKiem Or
				nxb.TENNXB Like @ChuoiTimKiem Or
				skd.TenSach Like @ChuoiTimKiem Or
				skd.TenChuDe Like @ChuoiTimKiem Or
				skd.TenNXB like @ChuoiTimKiem)
Go

ALTER function [dbo].[fn_ThongKeSLPhong](@namTK datetime)
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
						from PHONG p,CHITIETMUONPHONG ct,THETHUVIEN t
						where p.MAPHONG = ct.MAPHONG and t.MATTV = ct.MANSD and MONTH(ct.THOIGIANMUON) = @thang and DATEDIFF(YEAR,ct.THOIGIANMUON,'2022-03-06') =0
						group by year(ct.THOIGIANMUON)
						)
			set @kq = @kq + convert(varchar(50),@soPhong)+','
			set @thang = @thang +1
		end
		return @kq
	end
go
Alter Function FN_Ktrathoigian (@Map Int, @M Datetime, @T Datetime)
Returns Int
As
	Begin
		Declare @Kq Int
		Set @Kq = 0
		Declare CR_TG Cursor For
			Select THOIGIANMUON, THOIGIANTRA 
			From CHITIETMUONPHONG
			Where MAPHONG = @Map And Day(THOIGIANMUON) = Day(@M)
		Declare @Tgm Datetime,@Tgt Datetime
		Open CR_TG

		Fetch Next From CR_TG Into @Tgm,@Tgt
		While(@@FETCH_STATUS = 0)
		Begin
			If (Datediff(Minute, @M, @Tgt) >= 0 And DateDiff(Minute, @T, @Tgt) <= 0)
				Set @Kq = 1
			If (DateDiff(Minute, @M, @TgM) <= 0 And DateDiff(Minute, @T, @Tgt) >= 0)
				Set @Kq = 1
			If (Datediff(Minute, @T, @Tgt) >= 0 And DateDiff(Minute, @M, @Tgm) <= 0 And Datediff(Minute, @T, @Tgm) <= 0)
				Set @Kq = 1
			If (DateDiff(Minute, @M, @TgM) >= 0 And DateDiff(Minute, @T, @Tgt) >= 0 And Datediff(Minute, @T, @Tgm) < 0)
				Set @Kq = 1
			Fetch Next From CR_TG Into @Tgm,@Tgt
		End
		Close CR_TG
		Deallocate CR_TG
		Return @Kq
	End
Go
create proc SP_CapNhatTaiKhoan @maTK int, @tenDN varchar(30), @mK varchar(10), @chucVu nvarchar(30)
as
	SET XACT_ABORT ON
	begin tran
	update TAIKHOAN
	set TENDN = @tenDN,MATKHAU = @mK,CHUCVU = @chucVu
	where MATAIKHOAN = @maTK
	commit tran
go
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
---Khu vực test---
select * from F_TimPhongTrongTrongKhoangThoiGianAB ('2022-12-04 15:00:00','2022-12-04 17:00:00')
select * from F_TimPhongTrongTrongKhoangThoiGianAB ('2022-12-04 03:00:00','2022-12-04 04:00:00')
select * from CHITIETMUONPHONG

Insert into ThongBao(manguoitao, tieude, noidung, thoigian)
values(null, '123', '123', GETDATE())

select * from taikhoan
select * from nhanvien

insert into TAIKHOAN values(null, 'saTest', '', 'Admin')
insert into NHANVIEN values(25, 'Sysadmin', '1900-01-01', '0', '0', '0')
go
exec dbo.TaoTaiKhoanNhanVien 'Sysadmin', '1900-01-01', '0', '0', '0', 'saTest', '123', 'Admin'

------------------


--Log backup
USE [msdb]
GO
DECLARE @jobId BINARY(16)
EXEC  msdb.dbo.sp_add_job @job_name=N'BackUp_Log', 
		@enabled=1, 
		@notify_level_eventlog=0, 
		@notify_level_email=2, 
		@notify_level_page=2, 
		@delete_level=0, 
		@category_name=N'[Uncategorized (Local)]', 
		@owner_login_name=N'sa', @job_id = @jobId OUTPUT
select @jobId
GO
EXEC msdb.dbo.sp_add_jobserver @job_name=N'BackUp_Log', @server_name = N'DESKTOP-GUE0JS7'
GO
USE [msdb]
GO
EXEC msdb.dbo.sp_add_jobstep @job_name=N'BackUp_Log', @step_name=N'Log_BackUp', 
		@step_id=1, 
		@cmdexec_success_code=0, 
		@on_success_action=1, 
		@on_fail_action=2, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'backup log QLTHUVIEN
to disk = ''D:\QLTV_BACKUP\QLTV_Log.trn''', 
		@database_name=N'QLTHUVIEN', 
		@flags=0
GO
USE [msdb]
GO
EXEC msdb.dbo.sp_update_job @job_name=N'BackUp_Log', 
		@enabled=1, 
		@start_step_id=1, 
		@notify_level_eventlog=0, 
		@notify_level_email=2, 
		@notify_level_page=2, 
		@delete_level=0, 
		@description=N'', 
		@category_name=N'[Uncategorized (Local)]', 
		@owner_login_name=N'sa', 
		@notify_email_operator_name=N'', 
		@notify_page_operator_name=N''
GO
USE [msdb]
GO
DECLARE @schedule_id int
EXEC msdb.dbo.sp_add_jobschedule @job_name=N'BackUp_Log', @name=N'Weekly_BackUp_Log', 
		@enabled=1, 
		@freq_type=8, 
		@freq_interval=3, 
		@freq_subday_type=1, 
		@freq_subday_interval=0, 
		@freq_relative_interval=0, 
		@freq_recurrence_factor=1, 
		@active_start_date=20230103, 
		@active_end_date=99991231, 
		@active_start_time=200000, 
		@active_end_time=235959, @schedule_id = @schedule_id OUTPUT
select @schedule_id
GO



----
grant all on CHITIETMUONPHONG to QuanTriVien


create function FN_DanhSachTTV ()
returns table
as
	return 
	select * from THETHUVIEN