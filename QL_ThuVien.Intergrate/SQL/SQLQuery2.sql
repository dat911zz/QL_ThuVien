Select TABLE_NAME from Information_Schema.Tables
Select SPECIFIC_NAME From Information_Schema.Routines

Select * From Sys.Database_Principals
exec CapHuyQuyenBang 'NV3','CHUDE',1,1,1,1

Grant Select On NV
Create View DanhSachDoiTuong
As
	Select User_Name(MemberUID) As UserName
	From Sys.Sysmembers
	Where User_Name(MemberUID) <> 'dbo'
	Union
	Select Distinct User_Name(GroupUID) As RoleName
	From Sys.Sysmembers
	Where User_Name(GroupUID) <> 'db_owner'

	select * from DanhSachDoiTuong

exec KiemTraQuyen 'NV3','CHUDE'
alter Procedure KiemTraQuyen @target varchar(50), @object varchar(200)
As
	Begin
		SELECT  pri.name As Username, permit.permission_name AS [Permission],
				permit.state_desc AS [Permission State], object_name(permit.major_id) AS [Object Name]
		FROM    sys.database_principals pri LEFT JOIN sys.database_permissions permit ON permit.grantee_principal_id = pri.principal_id
		Where pri.name like @target And object_name(permit.major_id) like @object
	End
Go

Execute KiemTraQuyenBang 'NhanVien', 'Phong'

Alter Procedure CapHuyQuyenBang @target varchar(50), @object varchar(200), @Xem bit, @Xoa bit, @Them bit, @Sua bit
As
	Begin
		Declare @P_HuyQuyen varchar(200), @P_CapQuyen varchar(200), @GrantMoreThanOne bit = 0, @DenyMoreThanOne bit = 0

		Set @P_CapQuyen = 'Grant '
		Set @P_HuyQuyen = 'Deny '

		If(@Xem = 0)
			Begin
				Set @P_HuyQuyen = @P_HuyQuyen + 'Select '
				Set @DenyMoreThanOne = 1
			End
		Else
			Begin
				Set	@P_CapQuyen = @P_CapQuyen + 'Select '
				Set @GrantMoreThanOne = 1
			End

		If(@Xoa = 0)
			Begin
				If (@DenyMoreThanOne = 0)
					Begin
						Set @P_HuyQuyen = @P_HuyQuyen + 'Delete '
						Set @DenyMoreThanOne = 1
					End
				Else
					Set @P_HuyQuyen = @P_HuyQuyen + ', Delete '
			End
		Else
			Begin
				If (@GrantMoreThanOne = 0)
					Begin
						Set @P_CapQuyen = @P_CapQuyen + 'Delete '
						Set @GrantMoreThanOne = 1
					End
				Else
					Set @P_CapQuyen = @P_CapQuyen + ', Delete '
			End

		If(@Them = 0)
			Begin
				If (@DenyMoreThanOne = 0)
					Begin
						Set @P_HuyQuyen = @P_HuyQuyen + 'Insert '
						Set @DenyMoreThanOne = 1
					End
				Else
					Set @P_HuyQuyen = @P_HuyQuyen + ', Insert '
			End
		Else
			Begin
				If (@GrantMoreThanOne = 0)
					Begin
						Set @P_CapQuyen = @P_CapQuyen + 'Insert '
						Set @GrantMoreThanOne = 1
					End
				Else
					Set @P_CapQuyen = @P_CapQuyen + ', Insert '
			End

		If(@Sua = 0)
			Begin
				If (@DenyMoreThanOne = 0)
					Begin
						Set @P_HuyQuyen = @P_HuyQuyen + 'Update '
						Set @DenyMoreThanOne = 1
					End
				Else
					Set @P_HuyQuyen = @P_HuyQuyen + ', Update '
			End
		Else
			Begin
				If (@GrantMoreThanOne = 0)
					Begin
						Set @P_CapQuyen = @P_CapQuyen + 'Update '
						Set @GrantMoreThanOne = 1
					End
				Else
					Set @P_CapQuyen = @P_CapQuyen + ', Update '
			End

		Set @P_HuyQuyen += 'On ' + @object + ' To ' + @target
		Set @P_CapQuyen += 'On ' + @object + ' To ' + @target

		If(@GrantMoreThanOne = 1 And @DenyMoreThanOne = 1)
			Begin
				Execute (@P_HuyQuyen)
				Execute (@P_CapQuyen)
			End
		Else If (@GrantMoreThanOne = 1 And @DenyMoreThanOne = 0)
			Begin
				Execute (@P_CapQuyen)
			End
		Else If (@GrantMoreThanOne = 0 And @DenyMoreThanOne = 1)
			Begin
				Execute (@P_HuyQuyen)
			End
	End	
Go

alter Procedure KiemTraQuyen @target varchar(50), @object varchar(200)
As
	Begin
		SELECT  pri.name As Username, permit.permission_name AS [Permission],
				permit.state_desc AS [Permission State], object_name(permit.major_id) AS [Object Name]
		FROM    sys.database_principals pri LEFT JOIN sys.database_permissions permit ON permit.grantee_principal_id = pri.principal_id
		Where pri.name like @target And object_name(permit.major_id) like @object
	End
Go

Execute F_KiemTraBangThuTucHoacHam 'Phong'

alter Function F_KiemTraBangThuTucHoacHam (@object varchar(200))
Returns int
As
	Begin
		Declare @Bang bit, @ThuTuc bit, @HamVoHuong bit, @HamNoiTuyen bit, @LoaiObject tinyint = 4

		If (@object In (Select Table_Name From Information_Schema.Tables))
			Set @Bang = 1
		Else
			Set @Bang = 0

		If (@Bang = 0)
			Begin
				If (Select Top 1 ROUTINE_TYPE From Information_Schema.Routines Where SPECIFIC_NAME Like @object) Like 'PROCEDURE'
					Set @ThuTuc = 1
				Else
					Set @ThuTuc = 0

				If (Select Top 1 Data_Type From Information_Schema.Routines Where SPECIFIC_NAME Like @object) Like 'TABLE'
					Set @HamVoHuong = 0
				Else
					Set @HamVoHuong = 1
			End

		If(@Bang = 1)
			Set @LoaiObject = 0
		Else
			If (@ThuTuc = 1)
				Set @LoaiObject = 1
			Else
				If (@HamVoHuong = 0)
					Set @LoaiObject = 2
				Else
					Set @LoaiObject = 3
		Return @LoaiObject
	End
Go

Print dbo.F_KiemTraBangThuTucHoacHam ('ThemCotKhongDau')


Create Table LogTaiKhoanNhanVien (
	nguoidung varchar(50),
	thoigianthuchien DateTime,
	chitietcaulenh nvarchar(max)
)
Go

Create Procedure ThemLog @lenh nvarchar(Max)
As
	Begin
		Insert into LogTaiKhoanNhanVien Values (current_user, Getdate(), @lenh)
	End
Go