Imports MySql.Data.MySqlClient

Public Class MySQLCom
    Public Shared MyConn As New MySqlConnection("server=ISL-DEV1;user id=PAYC;password=PAYC@xs4db;
            persistsecurityinfo=True;allowzerodatetime=True;convertzerodatetime=True;port=3306;
            database=paymaster;defaultcommandtimeout=180")

    Public Structure InsStruct
        Shared TC_REC_NO As String
        Shared EMP_NUM As String
        Shared PAYROLL_DATE As String
        Shared PAYROLL_NUM As String
        Shared PAYPERIOD As String
        Shared PAY_ADJHRS As String
        Shared PAY_OTHRS As String
        Shared PAY_DTHRS As String
        Shared PAY_TYPE As String
        Shared PROLL_TEMP_ID As String
    End Structure
    '{-- Plan of Attack --}
    '1. Accumulate Hr/Daily/Weekly Totals by Employee
    '2. |->Check Employee Table for(EMP#): 
    '      Weekly/Daily Only -->Company#, Payperiod, PyrollBase(26|52), HrBase(2340|2912) 
    '3. |->Check payroll_tables_temp for the Last PayRoll # and Payroll Date
    '      using the COMPANY#|Payperiod
    '4.Insert Into time_clock (Emp#,PyRollDate,PRoll#,PyAdjHrs,PyOTHrs,PyDTHrs)
    Public Structure EmpStruct
        Shared PROLL_TEMP_ID As String
        Shared PAYROLL_DATE As String
        Shared LAST_PROLL_NO As String
        Shared COMPANY_NUM As String
        Shared PAYPERIOD As String
        Shared PAYBASE As String
        Shared HRBASE As String
    End Structure

    Public Shared Function CalculateHrs(ByVal PType As String,
                                        ByVal AccumHrs As Double) As Boolean
        'Dim IStruct As InsStruct
        'Dim EStruc As EmpStruct
        Dim HrBase As Double
        Dim TotHrWrk As Int32
        'Dim PAY_ADJHRS As String
        'Dim PAY_OTHRS As String
        'Dim PAY_DTHRS As String
        'Dim PAY_TYPE As String

        Select Case PType
            Case "Hourly Rate"
                'InsStruct.PAY_ADJHRS = AccumHrs.ToString
                InsStruct.PAY_ADJHRS = (((CInt(EmpStruct.HRBASE) / CInt(EmpStruct.PAYBASE)) - AccumHrs) * -1).ToString

            Case "Hourly time & half"
                InsStruct.PAY_OTHRS = AccumHrs.ToString

            Case "Hourly - Double Time"
                InsStruct.PAY_DTHRS = AccumHrs.ToString

            Case "Holiday Time & Half Rate" 'Holiday Time & Half Rate
                InsStruct.PAY_OTHRS = AccumHrs.ToString

            Case "Holiday Double Rate"
                InsStruct.PAY_DTHRS = AccumHrs.ToString

            Case "Weekly-Salary"
                If AccumHrs = 2 Then
                    InsStruct.PAY_ADJHRS = (0).ToString
                Else
                    InsStruct.PAY_ADJHRS = ((CInt(EmpStruct.HRBASE) / (CInt(EmpStruct.PAYBASE) / 2)) * -1).ToString
                End If

            Case "Day-Rate"
                HrBase = CInt(EmpStruct.HRBASE) / CInt(EmpStruct.PAYBASE)
                TotHrWrk = (CInt(AccumHrs) * 8)
                InsStruct.PAY_ADJHRS = ((CInt(HrBase) - TotHrWrk) * -1).ToString

        End Select
        Return True
    End Function

    Public Shared Sub GetEmpCalcParam(ByVal EmpNum As String,
                                      ByVal PType As String)
        'Chaa Creek Servername=DELL-DS
        'Dim eStruct As EmpStruct
        'Dim inStruct As InsStruct
        'Dim MyConn As New MySqlConnection("server=ISL-DEV1;user id=PAYC;password=PAYC@xs4db;
        '    persistsecurityinfo=True;allowzerodatetime=True;convertzerodatetime=True;port=3306;
        '    database=paymaster;defaultcommandtimeout=180")

        Dim MyComm As New MySqlCommand("SELECT MAX(pt.payroll_no) AS MaxPRoll,
                e.emp_num, e.f_name, e.m_name, e.l_name, e.company_name,
                e.pay_period, e.payroll_base, e.hr_base, pt.payroll_date, pt.payrolltemp_id
            FROM employee AS e
                INNER JOIN payroll_tables_temp AS pt ON pt.company_num = e.company_name 
                AND pt.payperiod = e.pay_period
                WHERE e.emp_num = " & EmpNum &
            " GROUP BY e.emp_num", MyConn)
        Try
            MyConn.Open()

        Catch ex As MySqlException
            MsgBox(ex.Message, vbExclamation, "Unable To Connect")
        End Try
        Try
            Dim Urdr As MySqlDataReader = MyComm.ExecuteReader
            While Urdr.Read
                EmpStruct.LAST_PROLL_NO = Urdr("MaxPRoll").ToString
                EmpStruct.PAYROLL_DATE = Urdr("payroll_date").ToString
                EmpStruct.PAYBASE = Urdr("payroll_base").ToString
                EmpStruct.HRBASE = Urdr("hr_base").ToString
                EmpStruct.PAYPERIOD = Urdr("pay_period").ToString
                EmpStruct.PROLL_TEMP_ID = Urdr("payrolltemp_id").ToString

                InsStruct.EMP_NUM = EmpNum
                InsStruct.PAYROLL_DATE = EmpStruct.PAYROLL_DATE
                InsStruct.PAYROLL_NUM = EmpStruct.LAST_PROLL_NO
                InsStruct.PAYPERIOD = EmpStruct.PAYPERIOD
                InsStruct.PAY_TYPE = PType
                InsStruct.PROLL_TEMP_ID = EmpStruct.PROLL_TEMP_ID
                'Call CalculateHrs(PType, AccumHrs)

            End While
            Urdr.Close()
            'Call InsertTClock()
            MyConn.Close()

        Catch ex As Exception
            MyConn.Close()
        End Try

        'If OpInt <> "0" Then
        '    Return OpInt
        'Else
        '    Return "0"
        'End If
    End Sub
    Public Shared Function InsertTClock() As Boolean

        'Dim MyConn As New MySqlConnection("server=ISL-DEV1;user id=PAYC;password=PAYC@xs4db;
        '    persistsecurityinfo=True;allowzerodatetime=True;convertzerodatetime=True;port=3308;
        '    database=paymaster;defaultcommandtimeout=180")
        Dim MyComm As New MySqlCommand("INSERT INTO time_clock (emp_num,payroll_date,
                payroll_num,payperiod,pay_adjhrs,pay_othrs,pay_dthrs) 
            VALUES ('" & InsStruct.EMP_NUM & "'," &
                      "'" & InsStruct.PAYROLL_DATE & "'," &
                        InsStruct.PAYROLL_NUM & "," &
                       "'" & InsStruct.PAYPERIOD & "'," &
                       CDbl(InsStruct.PAY_ADJHRS) & "," &
                        CDbl(InsStruct.PAY_OTHRS) & "," &
                        CDbl(InsStruct.PAY_DTHRS) & ") ", MyConn)

        Try
            MyConn.Open()
            MyComm.ExecuteNonQuery()
            MyConn.Close()
            Return True

        Catch ex As MySqlException
            MyConn.Close()
            Return False
        End Try

    End Function
    Public Shared Function CheckIfRecExists() As Boolean
        Dim OpInt As Int32

        Dim MyComm As New MySqlCommand("SELECT COUNT(tc_rec_no) AS c_int FROM time_clock 
            WHERE payroll_num = '" & InsStruct.PAYROLL_NUM & "' " &
            " AND payperiod = '" & InsStruct.PAYPERIOD & "' " &
            " AND emp_num = '" & InsStruct.EMP_NUM & "' ", MyConn)
        MyConn.Open()
        Try
            Dim Urdr As MySqlDataReader = MyComm.ExecuteReader
            While Urdr.Read
                OpInt = CInt(Urdr("c_int").ToString)

            End While
            Urdr.Close()
            MyConn.Close()

        Catch ex As Exception
            Return False
            MyConn.Close()
        End Try

        If OpInt <> 0 Then
            Return True
        Else
            Return False
        End If
    End Function
    Public Shared Function RemoveTClock() As Boolean

        'Dim MyConn As New MySqlConnection("server=ISL-DEV1;user id=PAYC;password=PAYC@xs4db;
        '    persistsecurityinfo=True;allowzerodatetime=True;convertzerodatetime=True;port=3308;
        '    database=paymaster;defaultcommandtimeout=180")
        Dim MyComm As New MySqlCommand("DELETE FROM time_clock " &
                                       " WHERE payperiod = '" & InsStruct.PAYPERIOD & "' " &
                                       " AND payroll_num = " & InsStruct.PAYROLL_NUM, MyConn)

        Try
            MyConn.Open()
            MyComm.ExecuteNonQuery()
            MyConn.Close()
            Return True

        Catch ex As MySqlException
            MyConn.Close()
            Return False
        End Try

    End Function
    Public Shared Sub ClearStructs()
        InsStruct.EMP_NUM = Nothing
        InsStruct.PAYPERIOD = Nothing
        InsStruct.PAYROLL_DATE = Nothing
        InsStruct.PAYROLL_NUM = Nothing
        InsStruct.PAY_TYPE = Nothing
        InsStruct.PAY_ADJHRS = "0"
        InsStruct.PAY_DTHRS = "0"
        InsStruct.PAY_OTHRS = "0"
    End Sub
End Class
