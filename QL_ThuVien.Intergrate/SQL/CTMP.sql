--FUNCTION
--hàm thống kê phòng chờ xác nhận 
create function FN_PhongChoXN()
returns table
as
	return 
	select * from CHITIETMUONPHONG where MANHANVIEN is null
go
select * from dbo.FN_PhongChoXN()
go

--Function
--Create Function F_TimPhongDuocMuonTrongKhoangThoiGianAB (@ThoiGianA DateTime, @ThoiGianB DateTime)
--Returns Table
--As
--	Return (Select * From ChiTietMuonPhong
--			Where ThoiGianMuon >= @ThoiGianA And ThoiGianTra <= @ThoiGianB)
--Go
--select * from F_TimPhongDuocMuonTrongKhoangThoiGianAB ('2022-12-30 8:13:00','2022-12-30 16:15:00')

--go
--Create Function F_TimPhongTrongTrongKhoangThoiGianAB (@ThoiGianA DateTime, @ThoiGianB DateTime)
--Returns Table
--As
--	Return (Select Distinct MaPhong From ChiTietMuonPhong
--			Where MaPhong Not In (Select MaPhong From CHITIETMUONPHONG
--								Where ThoiGianMuon >= @ThoiGianA And ThoiGianTra <= @ThoiGianB ))
--Go
select * from F_TimPhongTrongTrongKhoangThoiGianAB ('2022-12-30 8:13:00','2022-12-30 16:15:00')
go

--PROC

-- hàm thêm chi tiết mượn phòng 
--alter proc ADD_ChiTietMuonPhong @maNSD int, @maphong int, @manv int, @TGM datetime, @TGT datetime 
--as 
--begin 
--	insert into CHITIETMUONPHONG
--	values (@maNSD,@maphong,@manv,@TGM,@TGT)
--end 

--exec ADD_ChiTietMuonPhong '5', '1', null, N'2022-06-12T15:16:00.000',N'2022-06-12T16:15:00.000'

--cập nhật chi tiết mượn phòng 
go

exec UpdateChiTietMuonPhong '5', '1', null , N'2022-06-12T15:16:00.000',N'2022-06-12T16:15:00.000'

go
create proc DEL_ChiTietMuonPhong @maNSD int 
as 
begin 
	delete CHITIETMUONPHONG
	where MANGUOISUDUNG = @maNSD
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

insert into CHITIETMUONPHONG
values (1,1,3,CAST('2022-12-30 13:13:00' as datetime),CAST('2022-12-30 16:15:00' as datetime))

go
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

go
select * from CHITIETMUONPHONG
insert into CHITIETMUONPHONG
values(3, 1, null, '2023-01-02 15:00:00.000', '2023-01-02 17:00:00.000')
insert into CHITIETMUONPHONG
values(3, 1, null, '2023-01-03 15:00:00.000', '2023-01-03 17:00:00.000')
insert into CHITIETMUONPHONG
values(3, 1, null, '2023-01-04 15:00:00.000', '2023-01-04 17:00:00.000')
go

insert into CHITIETMUONPHONG
values (1,1,3,N'2023-12-30 13:13:00','2023-12-30 16:15:00')

go
--alter Proc Add_Chitietmuonphong @Mansd Int, @Maphong Int, @Tgm Datetime, @Tgt Datetime
--As 
--Begin 
--	Insert Into Chitietmuonphong
--	Values (@Mansd,@Maphong,@Tgm,@Tgt)
--End 
--Go

--Cập Nhật Chi Tiết Mượn Phòng 

alter Proc Updatechitietmuonphong @Mansd Int , @Maphong Int, @Manv Int, @Tgm Datetime, @Tgt Datetime
As 
Begin
	Update Chitietmuonphong 
	Set Manhanvien = @Manv, Thoigiantra = @Tgt
	Where Mansd = @Mansd And Maphong = @Maphong And Thoigianmuon = @Tgm
End
Go

alter Proc Del_Chitietmuonphong @Mansd Int, @Maphong Int, @Tgm Datetime
As 
	Begin 
		Delete From Chitietmuonphong
		Where Mansd = @Mansd And Maphong = @Maphong And Thoigianmuon = @Tgm
	End
Go

alter Proc Add_Chitietmuonphong @Mansd Int, @Maphong Int, @Tgm Datetime, @Tgt Datetime
As 
	Begin 
		Insert Into Chitietmuonphong (MaNSD, MaPhong, ThoiGianMuon, ThoiGianTra)
		Values (@Mansd,@Maphong,@Tgm,@Tgt)
	End 
Go
exec Add_Chitietmuonphong 1, 4, N'2022-09-02 8:13:00','2022-09-02 16:15:00'
go
alter Trigger TRG_Tgmuon On CHITIETMUONPHONG
Instead Of Insert
As
	Begin
		Declare @Map Int , @Tgm Datetime,@Tgt Datetime, @MaNSD int, @Kq Int = 0
		Set @Map = (Select MAPHONG From Inserted)
		Set @Tgm = (Select THOIGIANMUON From Inserted)
		Set @Tgt = (Select THOIGIANTRA From Inserted)
		Set @MaNSD = (Select MaNSD From Inserted)

		Set @Kq = (Select Dbo.FN_Ktrathoigian(@Map,@Tgm,@Tgt))

		If(@Kq = 1)
			Execute add_ChiTietMuonPhong @maNSD, @maP, @Tgm, @Tgt
			end
Go
Alter Function F_TimPhongTrongTrongKhoangThoiGianAB (@ThoiGianA DateTime, @ThoiGianB DateTime)
Returns Table
As
	Return (Select Distinct MaPhong From ChiTietMuonPhong
			Where dbo.FN_Ktrathoigian(maPhong, @ThoiGianA, @ThoiGianB) = 0 )
Go