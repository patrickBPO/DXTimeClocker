Imports System.ComponentModel
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports DXTimeClocker.MySQLCom
Imports MySql.Data.MySqlClient
'{-- Plan of Attack --}
'1. Accumulate Hr/Daily/Weekly Totals by Employee
'2. |->Check Employee Table for(EMP#): 
'      Weekly/Daily Only -->Company#, Payperiod, PyrollBase(26|52), HrBase(2340|2912) 
'3. |->Check payroll_tables_temp for the Last PayRoll # and Payroll Date
'      using the COMPANY#|Payperiod
'4.Insert Into time_clock (Emp#,PyRollDate,PRoll#,PyAdjHrs,PyOTHrs,PyDTHrs)
Partial Public Class s
    Dim open As New OpenFileDialog
    Dim PubRecCnt As Integer = 0
    Shared Sub New()
        DevExpress.UserSkins.BonusSkins.Register()
        DevExpress.Skins.SkinManager.EnableFormSkins()
    End Sub
    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub BtnFndFile_Click(sender As Object, e As EventArgs) Handles BtnFndFile.Click
        PBLoad.Hide()
        open.AutoUpgradeEnabled = False
        'open.Filter = "All Files|*.*"
        open.Filter = "Text files(*.txt, *.csv)|*.txt;*.csv"
        open.ShowDialog()
        TxtFilePath.Text = open.FileName.ToString
    End Sub

    Private Sub BtnLdFile_Click(sender As Object, e As EventArgs) Handles BtnLdFile.Click
        Dim cnt As Int32 = 0
        Dim lcnt As Int32 = 0
        Dim ChkCnt As Int32 = 0

        Dim EMP_NUM As String = Nothing '-- 1
        Dim PAYROLL_DATE As String = Nothing '-- 2
        Dim PAYROLL_NUM As String = Nothing '-- 3
        Dim TC_REC_NO As String = Nothing '-- 5
        Dim PAY_ADJHRS As String = Nothing '-- 8
        Dim PAY_OTHRS As String = Nothing '-- 9
        Dim PAY_DTHRS As String = Nothing '-- 10(Convert to Number)
        Dim PAY_TYPE As String = Nothing
        Dim SaveEmpNum As String = Nothing
        Dim SavePayType As String = Nothing
        '{-- Below used to Accumulate values based on Pay Type (e.g. Hourly Rate)
        Dim HrAccum As Double = 0.00
        Dim SpHrAccum As Double = 0.00
        Dim OTAccum As Double = 0.00
        Dim SpOTAccum As Double = 0.00
        Dim DTAccum As Double = 0.00
        Dim SpDTAccum As Double = 0.00
        Dim HOTAccum As Double = 0.00
        Dim HDTAccum As Double = 0.00
        Dim WkAccum As Double = 0.00
        Dim DyAccum As Double = 0.00
        Dim TotHrAccum As Double = 0.00
        Dim TotOTAccum As Double = 0.00
        Dim TotDTAccum As Double = 0.00
        Dim TotHOTAccum As Double = 0.00
        Dim TotHDTAccum As Double = 0.00
        Dim TotWkAccum As Double = 0
        Dim TotDyAccum As Double = 0
        ' Weekly- (2 recs: pay_adjhrs = 0) (1 recs: pay_adjhrs = HrBase/52 * -1 
        ' Daily- (pay_adjhrs = Record Count * 8 = TotHrsWrk, (((HrBase/52) * 2) - TotHrsWrk) * -1
        '--}
        Dim TargetPath As String
        'Dim TargetFile As String
        'Dim TargetLine As String
        'Dim OutputPath As String
        Dim MainPath As String
        Dim Conn As New MySqlConnection
        Dim Comm As New MySqlCommand

        Dim LineCount As Integer
        Dim i As Integer = 0
        Dim PbIncFactor As Integer

        Conn.ConnectionString = "server=localhost;user id=dmtsuser;password=Dmtsuser@xs4db;
            persistsecurityinfo=True;allowzerodatetime=True;convertzerodatetime=True;port=3308;
            database=dmts;defaultcommandtimeout=180"

        MainPath = Strings.Left(open.FileName.ToString, Len(open.FileName) - Len(open.SafeFileName))

        '-- Extract the main path(e.g. C:\...
        'TargetFile = MainPath & "file.csv"
        'TargetFile = "file.csv"
        TargetPath = "Completed\"
        'OutputPath = "Output\"
        Dim MyFname As String = open.FileName
        Dim ValidPath As String = "^(?:[\w]\:|\\)(\\[A-Za-z]_\-\s0-9\.]{0,})+\.(txt|gif|pdf|doc|docx|xls|xlsx)$"
        Dim ReValidPath As New Regex(ValidPath)

        MyFname = TxtFilePath.Text.Trim

        'Dim Myfile As System.IO.StreamWriter
        'Myfile = My.Computer.FileSystem.OpenTextFileWriter(MainPath & TargetFile, True)
        'Myfile = My.Computer.FileSystem.OpenTextFileWriter(TargetFile, True)
        'Console.WriteLine("LastName,FirstName,Address1_1 Street,City_1,State_1,CountryCode_1,ZipCode_1," &
        '    "Exp Month,Exp Year,AccountNumber_1,AccountType_1,AccountNumber_2,AccountType_2,Card Number_3,Email,Cell")

        'Myfile.WriteLine("LastName,FirstName,Address1_1 Street,City_1,State_1,CountryCode_1,ZipCode_1," &
        '    "Exp Month,Exp Year,AccountNumber_1,AccountType_1,AccountNumber_2,AccountType_2,Card Number_3,Email,Cell")
        Try
            'If Not ReValidPath.IsMatch(TextBox1.Text) Then

            '    MsgBox("Please specify a valid file...", vbExclamation, "No File Specified")
            '    TextBox1.Focus()
            '    open.FileName = ""
            '    Myfile.Close()
            '    Exit Sub

            'End If
            '{----------------------UNCOMMENT THIS BLOCK WHEN READY TO SET UP THE INSERT
            'Conn.Open()
            'Comm.Connection = Conn
            'Comm.CommandText = "INSERT INTO transactions (lt_rec_no,fh,trans_date,div_nbr,currency_code,amount,ct_rec_no) 
            'VALUES (@lt_rec_no,@fh,@trans_date,@div_nbr,@currency_code,@amount,@ct_rec_no)"
            'Comm.Prepare()
            'Comm.Parameters.AddWithValue("@lt_rec_no", TC_REC_NO)
            'Comm.Parameters.AddWithValue("@fh", EMP_NUM)
            'Comm.Parameters.AddWithValue("@trans_date", PAYROLL_DATE)
            'Comm.Parameters.AddWithValue("@div_nbr", PAYROLL_NUM)
            'Comm.Parameters.AddWithValue("@currency_code", PAY_ADJHRS)
            'Comm.Parameters.AddWithValue("@amount", PAY_OTHRS)
            'Comm.Parameters.AddWithValue("@ct_rec_no", PAY_DTHRS)
            '---------------------------------------------------------------------------}
            Try
                LineCount = File.ReadAllLines(MyFname).Count ' Get the Total Record Count
                PBLoad.Properties.Maximum = LineCount ' Apply to progress bar so it increments accordingly

            Catch ex As Exception
                MsgBox(ex.Message, vbExclamation, "File Not Found")
                Exit Sub
            End Try
            i = 0
            Timer1.Start()

            For Each line As String In File.ReadLines(MyFname) ' Loop over lines in file.

                If lcnt < LineCount Then

                    If lcnt = 1 Then
                        PBLoad.Show()
                        PBLoad.EditValue = 0
                        PbIncFactor = lcnt
                    End If
                    'PBLoad.Increment(PbIncFactor)
                    PBLoad.Properties.Step = PbIncFactor
                    PBLoad.PerformStep()
                    PBLoad.Update()
                Else
                    PBLoad.EditValue = LineCount
                    'Timer1.Stop()
                End If

                cnt = 1
                If lcnt > 0 Then
                    Dim itm = Split(line, ",", -1) ' Display the item.
                    For Each col In itm
                        'If cnt < 24 Then
                        '    'Console.Write(x.TrimEnd)
                        '    'Console.Write(",")

                        'Else
                        'Console.WriteLine(x.TrimEnd)
                        'End If

                        Select Case cnt
                            Case 2 '-- 2
                                '-- Convert a string Date to a specified format
                                'PAYROLL_DATE = Format(DateTime.Parse(col.TrimEnd), "yyyy-MM-dd")

                            Case 6 '-- 5
                                EMP_NUM = col.TrimEnd
                                If (Not IsNothing(SaveEmpNum) And SaveEmpNum <> EMP_NUM) Then
                                    TotHrAccum += HrAccum
                                    TotOTAccum += OTAccum
                                    TotDTAccum += DTAccum
                                    TotWkAccum += WkAccum
                                    TotDyAccum += DyAccum
                                    Debug.Print("Formula-->Emp#=" & SaveEmpNum & " TotHr=" & TotHrAccum & " TotOT=" & TotOTAccum)
                                    'Call GetEmpCalcParam(SaveEmpNum, PAY_TYPE)
                                    If TotHrAccum <> 0 Then
                                        Call GetEmpCalcParam(SaveEmpNum, "Hourly Rate")
                                        Call CalculateHrs("Hourly Rate", TotHrAccum)
                                    End If
                                    If TotOTAccum <> 0 Then '- Below also handles "Holiday Time & Half Rate"
                                        Call GetEmpCalcParam(SaveEmpNum, "Hourly time & half")
                                        Call CalculateHrs("Hourly time & half", TotOTAccum)
                                    End If
                                    If TotDTAccum <> 0 Then '- Below also handles "Holiday Double Rate"
                                        Call GetEmpCalcParam(SaveEmpNum, "Hourly - Double Time")
                                        Call CalculateHrs("Hourly - Double Time", TotDTAccum)
                                    End If
                                    If TotWkAccum <> 0 Then
                                        Call GetEmpCalcParam(SaveEmpNum, "Weekly-Salary")
                                        Call CalculateHrs("Weekly-Salary", TotWkAccum)
                                    End If
                                    If TotDyAccum <> 0 Then
                                        Call GetEmpCalcParam(SaveEmpNum, "Day-Rate")
                                        Call CalculateHrs("Day-Rate", TotDyAccum)
                                    End If
                                    If ChkCnt = 0 Then
                                        If CheckIfRecExists() Then
                                            ChkCnt += 1
                                            Dim YN = MsgBox("This Payperiod Already Appears Loaded...Clear and Load anyway?", vbYesNo, "Duplicate")
                                            If YN = MsgBoxResult.No Then
                                                For x = lcnt To 0 Step -1
                                                    PubRecCnt = x
                                                    TxtRecCnt.Text = PubRecCnt.ToString
                                                    TxtRecCnt.Refresh()
                                                    PBLoad.Properties.Step = PbIncFactor * -1
                                                    PBLoad.PerformStep()
                                                    PBLoad.Update()
                                                Next
                                                GoTo CANCELAR
                                            Else
                                                Call RemoveTClock()
                                            End If
                                        End If

                                    End If

                                    Call InsertTClock()
                                    Call ClearStructs()
                                    DyAccum = 0
                                    WkAccum = 0
                                    DTAccum = 0
                                    OTAccum = 0
                                    HrAccum = 0
                                    TotDyAccum = 0
                                    TotWkAccum = 0
                                    TotDTAccum = 0
                                    TotOTAccum = 0
                                    TotDTAccum = 0
                                    TotOTAccum = 0
                                    TotHrAccum = 0
                                End If
                            Case 7
                                PAY_TYPE = col.TrimEnd

                            Case 8 '-- 7
                                Dim SpHrs = Split(col.TrimEnd, ":", 2)

                                'If SaveEmpNum = EMP_NUM Then
                                Select Case PAY_TYPE
                                    Case "Hourly Rate"
                                        HrAccum = HrAccum + CInt(SpHrs.First) + CInt(SpHrs.Last) / 60

                                    Case "Hourly time & half"
                                        OTAccum = OTAccum + CInt(SpHrs.First) + CInt(SpHrs.Last) / 60

                                    Case "Hourly - Double Time"
                                        DTAccum = DTAccum + CInt(SpHrs.First) + CInt(SpHrs.Last) / 60

                                    Case "Holiday Time & Half Rate"
                                        OTAccum = OTAccum + CInt(SpHrs.First) + CInt(SpHrs.Last) / 60

                                    Case "Holiday Double Rate"
                                        DTAccum = DTAccum + CInt(SpHrs.First) + CInt(SpHrs.Last) / 60

                                    Case "Weekly-Salary"
                                        WkAccum = WkAccum + CInt(SpHrs.First) + CInt(SpHrs.Last) / 60

                                    Case "Day-Rate"
                                        DyAccum = DyAccum + CInt(SpHrs.First) + CInt(SpHrs.Last) / 60
                                End Select

                        End Select

                        cnt += 1
                    Next

                    '{-------------------------UNCOMMENT WHEN READY--------------------
                    ''''Try
                    ''''    'Comm.Parameters.AddWithValue("@lt_rec_no", TC_REC_NO)
                    ''''    'Comm.Parameters.AddWithValue("@fh", EMP_NUM)
                    ''''    'Comm.Parameters.AddWithValue("@trans_date", PAYROLL_DATE)
                    ''''    'Comm.Parameters.AddWithValue("@div_nbr", PAYROLL_NUM)
                    ''''    'Comm.Parameters.AddWithValue("@currency_code", PAY_ADJHRS)
                    ''''    'Comm.Parameters.AddWithValue("@amount", PAY_OTHRS)
                    ''''    'Comm.Parameters.AddWithValue("@ct_rec_no", PAY_DTHRS)

                    ''''    Comm.Parameters("@lt_rec_no").Value = TC_REC_NO
                    ''''    Comm.Parameters("@fh").Value = EMP_NUM
                    ''''    Comm.Parameters("@trans_date").Value = PAYROLL_DATE
                    ''''    Comm.Parameters("@div_nbr").Value = PAYROLL_NUM
                    ''''    Comm.Parameters("@currency_code").Value = PAY_ADJHRS
                    ''''    Comm.Parameters("@amount").Value = PAY_OTHRS
                    ''''    Comm.Parameters("@ct_rec_no").Value = PAY_DTHRS

                    ''''    If Not InsertTransPrep(Comm) Then
                    ''''        MsgBox("Record (" & lcnt & ") failed to load...", vbExclamation, "Insert Failed")
                    ''''        TxtFilePath.Focus()
                    ''''        open.FileName = ""
                    ''''        Conn.Close()
                    ''''        Exit Sub
                    ''''    Else
                    ''''        'Console.WriteLine("Records Entered (" & lcnt & ")")
                    ''''        '------------------TxtRecs.Clear()
                    ''''        '------------------TxtRecs.Text = lcnt
                    ''''        Me.Refresh()
                    ''''        'Console.Clear()
                    ''''    End If
                    ''''Catch ex As Exception
                    ''''    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    ''''End Try
                    '------------------------------------------------------------------}
                    'If Not InsertTransPrep(UseInsStruct, Conn) Then
                    '    MsgBox("Record (" & lcnt & ") failed to load...", vbExclamation, "Insert Failed")
                    '    TextBox1.Focus()
                    '    open.FileName = ""
                    '    Exit Sub
                    'Else
                    '    'Console.WriteLine("Records Entered (" & lcnt & ")")
                    '    'Console.Clear()
                    'End If

                End If
                'TargetLine = String.Concat(LastName, ",", FirstName, ",", Street, ",", City, ",",
                '              State, ",", CountryCode, ",", ZipCode, ",", Mnth, ",", Yr, ",",
                '              AccountNumber_1, ",", AccountType_1, ",", AccountNumber_2, ",",
                '              AccountType_2, ",", CardNo, ",", Email, ",", Cell)
                ''Myfile.WriteLine(LastName & "," & FirstName & "," & Street & "," & City & ",")
                'Myfile.WriteLine(TargetLine)

                lcnt += 1
                PubRecCnt = lcnt
                TxtRecCnt.Text = PubRecCnt.ToString
                TxtRecCnt.Refresh()

                Debug.Print(EMP_NUM & "-" & SaveEmpNum & "HrAmt=" & HrAccum & " OTAmt=" & OTAccum & " " & line)
                SaveEmpNum = EMP_NUM
            Next
CANCELAR:
        Catch ex As FileNotFoundException
            MsgBox(ex.Message, vbExclamation, "File Not Found")
            TxtFilePath.Focus()
            open.FileName = ""
            Conn.Close()
            'Myfile.Close()
            Exit Sub
        End Try
        Timer1.Stop()
        Conn.Close()
        'Myfile.Close()
        '-- Move the TXT to the completed directory
        'My.Computer.FileSystem.MoveFile(MyFname, MainPath & TargetPath & open.SafeFileName,
        'FileIO.UIOption.AllDialogs,
        'FileIO.UICancelOption.ThrowException)
        '-- Move the CSV to the output directory
        'My.Computer.FileSystem.MoveFile(MainPath & TargetFile, MainPath & OutputPath & TargetFile,
        '        FileIO.UIOption.AllDialogs,
        '        FileIO.UICancelOption.ThrowException)
        '''''MsgBox("Loading(" & lcnt & ") Records Complete...Check [ " & MainPath & TargetPath & " ] for Processed Files", vbOK, "Loading Process Complete")
        TxtFilePath.Clear()
        open.FileName = ""
        BtnFndFile.Focus()
    End Sub

    Private Sub s_Load(sender As Object, e As EventArgs) Handles Me.Load
        PBLoad.Hide()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        'TxtRecCnt.Text = PubRecCnt.ToString
    End Sub

    Private Sub TxtFilePath_KeyUp(sender As Object, e As KeyEventArgs) Handles TxtFilePath.KeyUp
        Dim txtBx As TextBox = TryCast(sender, TextBox)
        If Not IsNothing(e.KeyValue) Then
            If txtBx.Text.Trim <> "" Then
                BtnLdFile.Enabled = True
            Else
                BtnLdFile.Enabled = False
            End If
        End If
    End Sub

    Private Sub TxtFilePath_TextChanged(sender As Object, e As EventArgs) Handles TxtFilePath.TextChanged
        If Len(TxtFilePath.Text) > 0 And Trim(TxtFilePath.Text) <> "" Then
            BtnLdFile.Enabled = True
        Else
            BtnLdFile.Enabled = False
        End If
    End Sub
End Class
