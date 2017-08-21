'类名：OdbcDbAccess 
'功能：使用Odbc链接数据库的操作类
'作者：陈晓雨
'编写时间：2007-6-20

Imports System.Data
Imports System.Data.Odbc
Imports System.Data.SqlClient
Imports System.Configuration

''' <summary>
''' Odbc数据库操作类
''' </summary>
''' <remarks></remarks>
Public Class OdbcDbAccess

#Region "字段"
    Private Shared ReadOnly _Connstr = System.Configuration.ConfigurationManager.ConnectionStrings("OdbcConnString").ToString.Trim
    Private Shared parmCache As Hashtable = Hashtable.Synchronized(New Hashtable)
#End Region

#Region "属性"
    ''' <summary>
    ''' 链接字符串
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property ConnStr() As String
        Get
            Return _Connstr
        End Get
    End Property
#End Region

#Region "方法"

    ''' <summary>
    ''' 得到默认的数据库链接对象
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' e.g.:dim conn as OdbcConnection = OdbcDbAccess.GetOdbConnection()
    ''' 得到一个ODBC数据库链接对象 链接字符串为Web.Config文件中的ConnectionStrings("OdbcConnString")节点
    ''' </remarks>
    Public Shared Function GetOdbConnection() As OdbcConnection
        Return New OdbcConnection(ConnStr)
    End Function
    ''' <summary>
    ''' 得到链接字符串的数据库链接对象没啥用
    ''' </summary>
    ''' <remarks>
    ''' e.g.: dim conn as new OdbcConnection = odbcdbaccess.GetOdbConnection("链接字符串...")
    '''       dim conn as new OdbcConnection("链接字符串...")
    ''' 两者一样的
    ''' </remarks>      
    ''' <param name="ConnString">一个合法的链接字符串</param>
    ''' <returns></returns>
    Public Shared Function GetOdbConnection(ByVal ConnString As String) As OdbcConnection
        Return New OdbcConnection(ConnString)
    End Function

    ''' <summary>
    ''' 执行没有返回结果的查询 
    ''' </summary>
    ''' <param name="ConnString">合法数据库链接字符串</param>
    ''' <param name="cmdType">查询类型T-SQL语句\存储过程</param>
    ''' <param name="cmdText">查询语句</param>
    ''' <param name="params">查询参数数组</param>
    ''' <returns>返回所影响的行数</returns>
    ''' <remarks></remarks>
    Public Shared Function ExecNoQuery(ByVal ConnString As String, ByVal cmdType As CommandType, ByVal cmdText As String, ByVal ParamArray params() As OdbcParameter) As Integer
        Dim cmd As New OdbcCommand
        Using con As New OdbcConnection(ConnString)
            PreparativeCommand(cmd, con, Nothing, cmdType, cmdText, params)
            Dim val As Integer = cmd.ExecuteNonQuery()
            cmd.Parameters.Clear()
            Return val
        End Using
    End Function
    ''' <summary>
    ''' 执行没有返回结果的查询 
    ''' </summary>
    ''' <param name="Conn">已存在的数据库链接对象</param>
    ''' <param name="cmdType">查询类型T-SQL语句\存储过程</param>
    ''' <param name="cmdText">查询语句</param>
    ''' <param name="params">查询参数数组</param>
    ''' <returns>返回所影响的行数</returns>
    ''' <remarks></remarks>
    Public Shared Function ExecNoQuery(ByVal Conn As OdbcConnection, ByVal cmdType As CommandType, ByVal cmdText As String, ByVal ParamArray params() As OdbcParameter) As Integer
        Dim cmd As New OdbcCommand
        Dim val As Integer
        PreparativeCommand(cmd, Conn, Nothing, cmdType, cmdText, params)
        val = cmd.ExecuteNonQuery()
        cmd.Parameters.Clear()
        Return val
    End Function
    ''' <summary>
    ''' 执行没有返回结果的查询  
    ''' </summary>
    ''' <param name="trans">已经存在的数据库事务</param>
    ''' <param name="cmdType">查询类型T-SQL语句\存储过程</param>
    ''' <param name="cmdText">查询语句</param>
    ''' <param name="params">查询参数数组</param>
    ''' <returns>返回所影响的行数</returns>
    ''' <remarks></remarks>
    Public Shared Function ExecNoQuery(ByVal trans As OdbcTransaction, ByVal cmdType As CommandType, ByVal cmdText As String, ByVal ParamArray params() As OdbcParameter) As Integer
        Dim cmd As New OdbcCommand
        PreparativeCommand(cmd, trans.Connection, trans, cmdType, cmdText, params)
        Dim val As Integer = cmd.ExecuteNonQuery()
        cmd.Parameters.Clear()
        Return val
    End Function
    ''' <summary>
    ''' 执行没有返回结果的查询  使用默认的数据库链接
    ''' </summary>
    ''' <param name="cmdType">查询类型T-SQL语句\存储过程</param>
    ''' <param name="cmdText">查询语句</param>
    ''' <param name="params">查询参数数组</param>
    ''' <returns>返回所影响的行数</returns>
    ''' <remarks></remarks>
    Public Shared Function ExecNoQuery(ByVal cmdType As CommandType, ByVal cmdText As String, ByVal ParamArray params() As OdbcParameter) As Integer
        Dim cmd As New OdbcCommand
        Using conn As New OdbcConnection(ConnStr)
            PreparativeCommand(cmd, conn, Nothing, cmdType, cmdText, params)
            Dim val As Integer = cmd.ExecuteNonQuery()
            cmd.Parameters.Clear()
            Return val
        End Using
    End Function

    ''' <summary>
    ''' 查询 返回数据流
    ''' </summary>
    ''' <param name="ConnString">一个合法的链接字符串</param>
    ''' <param name="cmdType">查询类型T-SQL语句\存储过程</param>
    ''' <param name="cmdText">查询语句</param>
    ''' <param name="params">查询参数</param>
    ''' <returns>数据流</returns>
    ''' <remarks></remarks>
    Public Shared Function ExecuteReader(ByVal ConnString As String, ByVal cmdType As CommandType, ByVal cmdText As String, ByVal ParamArray params() As OdbcParameter) As OdbcDataReader
        Dim cmd As New OdbcCommand
        Dim odbcReader As OdbcDataReader
        Using conn As New OdbcConnection(ConnString)
            PreparativeCommand(cmd, conn, Nothing, cmdType, cmdText, params)
            Try
                odbcReader = cmd.ExecuteReader(CommandBehavior.CloseConnection)
            Catch ex As Exception
                Throw ex
            Finally
                conn.Close()
            End Try
            cmd.Parameters.Clear()
            Return odbcReader
        End Using
    End Function
    ''' <summary>
    ''' 查询 使用默认的数据库链接 返回数据流
    ''' </summary>
    ''' <param name="cmdType">查询类型T-SQL语句\存储过程</param>
    ''' <param name="cmdText">查询语句</param>
    ''' <param name="params">查询参数</param>
    ''' <returns>数据流</returns>
    ''' <remarks></remarks>
    Public Shared Function ExecuteReader(ByVal cmdType As CommandType, ByVal cmdText As String, ByVal ParamArray params() As OdbcParameter) As OdbcDataReader
        Dim cmd As New OdbcCommand
        Dim odbcReader As OdbcDataReader
        Using conn As New OdbcConnection(ConnStr)
            PreparativeCommand(cmd, conn, Nothing, cmdType, cmdText, params)
            Try
                odbcReader = cmd.ExecuteReader(CommandBehavior.CloseConnection)
            Catch ex As Exception
                Throw ex
            Finally
                conn.Close()
            End Try
            cmd.Parameters.Clear()
            Return odbcReader
        End Using
    End Function

    ''' <summary>
    ''' 查询数据 使用默认的数据库链接 返回第一行的第一列
    ''' </summary>
    ''' <param name="cmdType">查询类型T-SQL语句\存储过程</param>
    ''' <param name="cmdText">查询语句</param>
    ''' <param name="params">查询参数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ExecuteScalar(ByVal cmdType As CommandType, ByVal cmdText As String, ByVal ParamArray params() As OdbcParameter) As Object
        Dim cmd As New OdbcCommand
        Using conn As New OdbcConnection(ConnStr)
            PreparativeCommand(cmd, conn, Nothing, cmdType, cmdText, params)
            Dim val As Object = cmd.ExecuteScalar()
            cmd.Parameters.Clear()
            Return val
        End Using

    End Function
    ''' <summary>
    ''' 查询数据 返回第一行的第一列
    ''' </summary>
    ''' <param name="cmdType">查询类型T-SQL语句\存储过程</param>
    ''' <param name="cmdText">查询语句</param>
    ''' <param name="params">查询参数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ExecuteScalar(ByVal ConnString As String, ByVal cmdType As CommandType, ByVal cmdText As String, ByVal ParamArray params() As OdbcParameter) As Object
        Dim cmd As New OdbcCommand
        Using conn As New OdbcConnection(ConnString)
            PreparativeCommand(cmd, conn, Nothing, cmdType, cmdText, params)
            Dim val As Object = cmd.ExecuteScalar()
            cmd.Parameters.Clear()
            Return val
        End Using

    End Function

    ''' <summary>
    ''' 查询数据 返回查询结果集
    ''' </summary>
    ''' <param name="ConnString">一个合法的链接字符串</param>
    ''' <param name="cmdType">查询类型T-SQL语句\存储过程</param>
    ''' <param name="cmdText">查询语句</param>
    ''' <param name="params">查询参数</param>
    ''' <returns>DataSet</returns>
    ''' <remarks></remarks>
    Public Shared Function GetDataSet(ByVal ConnString As String, ByVal cmdType As CommandType, ByVal cmdText As String, ByVal ParamArray params() As OdbcParameter) As DataSet
        Dim cmd As New OdbcCommand
        Dim ds As New DataSet
        Using conn As New OdbcConnection(ConnString)
            PreparativeCommand(cmd, conn, Nothing, cmdType, cmdText, params)
            Dim adapter As New OdbcDataAdapter(cmd)
            adapter.Fill(ds)
            cmd.Parameters.Clear()
            Return ds
        End Using
    End Function
    ''' <summary>
    ''' 查询数据 使用默认的数据库链接 返回查询结果集
    ''' </summary>
    ''' <param name="cmdType">查询类型T-SQL语句\存储过程</param>
    ''' <param name="cmdText">查询语句</param>
    ''' <param name="params">查询参数</param>
    ''' <returns>DataSet</returns>
    ''' <remarks></remarks>
    Public Shared Function GetDataSet(ByVal cmdType As CommandType, ByVal cmdText As String, ByVal ParamArray params() As OdbcParameter) As DataSet
        Dim cmd As New OdbcCommand
        Dim ds As New DataSet
        Using conn As New OdbcConnection(ConnStr)
            PreparativeCommand(cmd, conn, Nothing, cmdType, cmdText, params)
            Dim adapter As New OdbcDataAdapter(cmd)
            adapter.Fill(ds)
            cmd.Parameters.Clear()
            Return ds
        End Using
    End Function

    ''' <summary>
    ''' 查询数据 返回查询结果集
    ''' </summary>
    ''' <param name="ConnString">一个合法的链接字符串</param>
    ''' <param name="cmdType">查询类型T-SQL语句\存储过程</param>
    ''' <param name="cmdText">查询语句</param>
    ''' <param name="params">查询参数</param>
    ''' <returns>DataSet</returns>
    ''' <remarks></remarks>
    Public Shared Function GetDataTable(ByVal ConnString As String, ByVal cmdType As CommandType, ByVal cmdText As String, ByVal ParamArray params() As OdbcParameter) As DataTable
        Dim cmd As New OdbcCommand
        Dim table As New DataTable
        Using conn As New OdbcConnection(ConnString)
            PreparativeCommand(cmd, conn, Nothing, cmdType, cmdText, params)
            Dim adapter As New OdbcDataAdapter(cmd)
            adapter.Fill(table)
            cmd.Parameters.Clear()
            Return table
        End Using
    End Function
    ''' <summary>
    ''' 查询数据 使用默认的数据库链接 返回查询结果集
    ''' </summary>
    ''' <param name="cmdType">查询类型T-SQL语句\存储过程</param>
    ''' <param name="cmdText">查询语句</param>
    ''' <param name="params">查询参数</param>
    ''' <returns>DataSet</returns>
    ''' <remarks></remarks>
    Public Shared Function GetDataTable(ByVal cmdType As CommandType, ByVal cmdText As String, ByVal ParamArray params() As OdbcParameter) As DataTable
        Dim cmd As New OdbcCommand
        Dim table As New DataTable
        Using conn As New OdbcConnection(ConnStr)
            PreparativeCommand(cmd, conn, Nothing, cmdType, cmdText, params)
            Dim adapter As New OdbcDataAdapter(cmd)
            adapter.Fill(table)
            cmd.Parameters.Clear()
            Return table
        End Using
    End Function


    ''' <summary>
    ''' 得到某表的一空行
    ''' </summary>
    ''' <param name="ConnString">链接字符串</param>
    ''' <param name="TableName">数据库表名</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function RowBlankGet(ByVal ConnString As String, ByVal TableName As String) As DataRow
        Dim cmd As New OdbcCommand
        Dim ds As New DataSet
        Using conn As New OdbcConnection(ConnString)
            PreparativeCommand(cmd, conn, Nothing, CommandType.Text, "SELECT * FROM " & TableName & " WHERE 1=2")
            Dim adapter As New OdbcDataAdapter(cmd)
            adapter.Fill(ds)
            Return ds.Tables(0).NewRow()
        End Using
    End Function
    ''' <summary>
    ''' 得到某表的一空行 使用默认的数据库链接
    ''' </summary>
    ''' <param name="TableName">数据库表名</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function RowBlankGet(ByVal TableName As String) As DataRow
        Dim cmd As New OdbcCommand
        Dim ds As New DataSet
        Using conn As New OdbcConnection(ConnStr)
            PreparativeCommand(cmd, conn, Nothing, CommandType.Text, "SELECT * FROM " & TableName & " WHERE 1=2")
            Dim adapter As New OdbcDataAdapter(cmd)
            adapter.Fill(ds)
            Return ds.Tables(0).NewRow()
        End Using
    End Function


    ''' <summary>
    ''' 更新表的某一行
    ''' </summary>
    ''' <param name="ConnString">链接字符串</param>
    ''' <param name="RowID">行ID</param>
    ''' <param name="TableName">数据表名</param>
    ''' <param name="CurRow">新的数据行</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function RowUpdate(ByVal ConnString As String, ByVal RowID As String, ByVal TableName As String, ByVal CurRow As DataRow) As Boolean
        Dim cmd As New OdbcCommand
        Dim ds As New DataSet
        Dim tempRow As DataRow
        Using conn As New OdbcConnection(ConnString)

            '准备
            PreparativeCommand(cmd, conn, Nothing, CommandType.Text, "SELECT * FROM " & TableName & " WHERE ID=" & RowID)
            Dim adapter As New OdbcDataAdapter(cmd)          '适配器
            Dim builder As New OdbcCommandBuilder(adapter)

            '填充数据
            adapter.Fill(ds)

            '如果没有返回结果 就是改ID不存在
            If ds.Tables(0).Rows.Count = 0 Then
                Return False
            End If

            '开始编辑行
            tempRow = ds.Tables(0).Rows(0)
            tempRow.BeginEdit()
            '两行数据质检的赋值
            Try
                RowItemCopy(tempRow, CurRow)
            Catch ex As Exception
                Return False
            End Try
            'ID
            tempRow.Item("id") = RowID

            '结束编辑
            tempRow.EndEdit()
            '更新
            adapter.Update(ds)
            Return True
        End Using
    End Function
    ''' <summary>
    ''' 更新表的某一行  使用默认的数据库链接
    ''' </summary>
    ''' <param name="RowID">行ID</param>
    ''' <param name="TableName">数据表名</param>
    ''' <param name="CurRow">新的数据行</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function RowUpdate(ByVal RowID As String, ByVal TableName As String, ByVal CurRow As DataRow) As Boolean
        Dim cmd As New OdbcCommand
        Dim ds As New DataSet
        Dim tempRow As DataRow
        Using conn As New OdbcConnection(ConnStr)

            '准备
            PreparativeCommand(cmd, conn, Nothing, CommandType.Text, "SELECT * FROM " & TableName & " WHERE ID=" & RowID)
            Dim adapter As New OdbcDataAdapter(cmd)          '适配器
            Dim builder As New OdbcCommandBuilder(adapter)

            '填充数据
            adapter.Fill(ds)

            '如果没有返回结果 就是改ID不存在
            If ds.Tables(0).Rows.Count = 0 Then
                Return False
            End If

            '开始编辑行
            tempRow = ds.Tables(0).Rows(0)
            tempRow.BeginEdit()
            '两行数据质检的赋值
            Try
                RowItemCopy(tempRow, CurRow)
            Catch ex As Exception
                Return False
            End Try
            'ID
            tempRow.Item("id") = RowID

            '结束编辑
            tempRow.EndEdit()
            '更新
            adapter.Update(ds)
            Return True
        End Using
    End Function


    ''' <summary>
    ''' 向数据表中添加一条新的记录 并返回ID
    ''' </summary>
    ''' <param name="ConnString">链接字符串</param>
    ''' <param name="TableName">表名</param>
    ''' <param name="RowObj">数据行</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function RowAddNew(ByVal ConnString As String, ByVal TableName As String, ByVal RowObj As DataRow) As Integer
        Dim cmd As New OdbcCommand
        Dim ds As New DataSet
        Using conn As New OdbcConnection(ConnString)
            PreparativeCommand(cmd, conn, Nothing, CommandType.Text, "SELECT * FROM " & TableName & " WHERE 1=2")
            Dim adapter As New OdbcDataAdapter(cmd)          '适配器
            Dim builder As New OdbcCommandBuilder(adapter)

            '填充数据
            adapter.Fill(ds)
            '添加新行
            ds.Tables(0).Rows.Add(RowObj)
            '更新
            adapter.Update(ds)
            '从新填充
            adapter.Fill(ds)
            ' 返回更新后的ID
            Return ds.Tables(0).Rows(0).Item("ID")
        End Using
    End Function
    ''' <summary>
    ''' 向数据表中添加一条新的记录 并返回ID(使用默认的数据库链接)
    ''' </summary>
    ''' <param name="TableName">表名</param>
    ''' <param name="RowObj">数据行</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function RowAddNew(ByVal TableName As String, ByVal RowObj As DataRow) As Integer
        Dim cmd As New OdbcCommand
        Dim ds As New DataSet
        Using conn As New OdbcConnection(ConnStr)
            PreparativeCommand(cmd, conn, Nothing, CommandType.Text, "SELECT * FROM " & TableName & " WHERE 1=2")
            Dim adapter As New OdbcDataAdapter(cmd)          '适配器
            Dim builder As New OdbcCommandBuilder(adapter)

            '填充数据
            adapter.Fill(ds)
            '添加新行
            ds.Tables(0).Rows.Add(RowObj)
            '更新
            adapter.Update(ds)
            '从新填充
            adapter.Fill(ds)
            ' 返回更新后的ID
            Return ds.Tables(0).Rows(0).Item("ID")
        End Using
    End Function

    ''' <summary>
    ''' 为执行T-SQL语句做准备
    ''' </summary>
    ''' <param name="cmd">OledbCommand 对象</param>
    ''' <param name="conn">数据库链接对象</param>
    ''' <param name="trans">数据库操作事务</param>
    ''' <param name="cmdType">查询类型</param>
    ''' <param name="cmdText">查询语句</param>
    ''' <param name="parms">查询需要的参数数组</param>
    ''' <remarks></remarks>
    Private Shared Sub PreparativeCommand(ByVal cmd As OdbcCommand, ByVal conn As OdbcConnection, ByVal trans As OdbcTransaction, ByVal cmdType As CommandType, ByVal cmdText As String, ByVal ParamArray parms() As OdbcParameter)

        '如果链接没有打开 打开链接
        If conn.State <> ConnectionState.Open Then
            conn.Open()
        End If

        '设置数据库链接
        cmd.Connection = conn
        '查询语句
        cmd.CommandText = cmdText
        '查询类型  T-SQL \ 存储过程
        cmd.CommandType = cmdType

        '是使用事务提交还是不使用
        If Not IsNothing(trans) Then
            cmd.Transaction = trans
        End If

        '如果查询参数不为空(给参数赋值)
        If Not IsNothing(parms) Then
            For Each par As OdbcParameter In parms
                cmd.Parameters.Add(par)
            Next
        End If
    End Sub

    ''' <summary>
    ''' 两个数据行之间的赋值
    ''' </summary>
    ''' <param name="DestRow"></param>
    ''' <param name="SrcRow"></param>
    ''' <remarks></remarks>
    Private Sub RowItemCopy(ByVal DestRow As DataRow, ByVal SrcRow As DataRow)
        Dim i As Integer
        '对每项数据进行复制
        Try
            For i = 0 To DestRow.ItemArray.Length - 1
                DestRow.Item(i) = SrcRow.Item(i)
            Next
        Catch ex As Exception
            Throw ex
            Exit Sub
        End Try

    End Sub

    '''<summary>
    '''缓存查询参数
    ''' </summary>
    ''' <param name="cacheKey">名字</param>
    ''' <param name="parms">参数数组</param>
    ''' <remarks>
    ''' 保存需要缓存的参数数组
    ''' </remarks>
    Public Shared Sub CacheParameters(ByVal cacheKey As String, ByVal ParamArray parms() As OdbcParameter)
        parmCache(cacheKey) = parms
    End Sub
    ''' <summary>
    ''' 得到缓存的参数
    ''' </summary>
    ''' <param name="cacheKey">名字</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetCachedParameters(ByVal cacheKey As String) As OdbcParameter()
        Dim params() As OdbcParameter = CType(parmCache(cacheKey), OdbcParameter())
        If params Is Nothing Then
            Return Nothing
        End If

        Dim clonedParms(params.Length - 1) As OdbcParameter
        For i As Integer = 0 To params.Length - 1
            clonedParms(i) = CType(CType(params(i), ICloneable).Clone, OdbcParameter)
        Next
        Return clonedParms
    End Function
#End Region

End Class
