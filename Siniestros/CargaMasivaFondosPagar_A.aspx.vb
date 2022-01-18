
Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization
Imports Mensaje
Imports System.Data.OleDb

Partial Class Siniestros_OrdenPago
    Inherits System.Web.UI.Page

#Region "Declaración de variables"
    Public Property dtImportaReasPrim() As DataTable
        Get
            Return Session("dtImportaReasPrim")
        End Get
        Set(ByVal value As DataTable)
            Session("dtImportaReasPrim") = value
        End Set
    End Property
    Public Property dtReasPrim() As DataTable
        Get
            Return Session("dtReasPrim")
        End Get
        Set(ByVal value As DataTable)
            Session("dtReasPrim") = value
        End Set
    End Property


#End Region

#Region "Eventos"
    Sub Page_Load(ByVal Sender As Object, ByVal e As EventArgs) Handles Me.Load

        'Leercandado()

    End Sub

#End Region

#Region "Funciones"

#End Region

#Region "Métodos"


    Private Sub btn_CargaLayoutIngPrimas_Click(sender As Object, e As EventArgs) Handles btn_CargaLayoutIngPrimas.Click

        Try
            CrearTablaImportarReasPrimas()
            Dim oConn As New OleDbConnection
            Dim filename = System.IO.Path.GetFileName(fiu_ReasPrim.FileName)
            Dim directorio As String = Funciones.fn_Parametro_Directo(55)
            fiu_ReasPrim.SaveAs(directorio + filename)
            Dim Ruta As String = vbNullString
            Ruta = directorio + filename
            Dim extecion As String = System.IO.Path.GetExtension(fiu_ReasPrim.FileName)
            Dim oCmd As New OleDbCommand
            Dim oDa As New OleDbDataAdapter
            Dim oDs As New DataSet
            Dim strConn As String = ""

            If extecion = ".xls" Then
                strConn = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES' ", Ruta)
            ElseIf extecion = ".xlsx" Then
                strConn = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0;XML;HDR=YES;IMEX=2' ", Ruta)
            End If

            Dim cn As New OleDb.OleDbConnection(strConn) 'cadena de coneccion

            oCmd.Connection = cn
            oCmd.CommandText = "select Folio_Onbase,Pagar_A from [Folios$]"
            oCmd.CommandType = CommandType.Text
            oDa.SelectCommand = oCmd
            oDa.Fill(dtImportaReasPrim)
            gvd_CargaMasivaIngPrimas.DataSource = dtImportaReasPrim
            gvd_CargaMasivaIngPrimas.DataBind()


            Mensaje.MuestraMensaje(Master.Titulo, "Archivo importado!!!", TipoMsg.Confirma)

        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "fn_CargaLayoutReas: " & ex.Message)
        End Try
    End Sub
#End Region
    Private Sub btn_CargarLayout_Click(sender As Object, e As EventArgs) Handles btn_CargarLayout.Click
        Try
            Funciones.AbrirModal("#CargaLayoutIngresosPrimas")
        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "btn_CargarLayout_Click: " & ex.Message)
        End Try
    End Sub

    Public Sub CrearTablaImportarReasPrimas()
        dtImportaReasPrim = New DataTable()
        dtImportaReasPrim.Columns.Add("Folio_Onbase", GetType(Integer))
        dtImportaReasPrim.Columns.Add("Pagar_A")

    End Sub
    Private Sub btn_ValidarReas_Click(sender As Object, e As EventArgs) Handles btn_ValidarReas.Click
        Try
            ' If dtReasPrim.Rows.Count = 0 Then
            fn_ValidarCargaMasivaReasPrimas()
                'fn_ActualizaSaldoReasPrim()
            'End If
        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, ex.Message, TipoMsg.Falla)
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "btn_ValidarReas_Click: " & ex.Message)
        End Try
    End Sub
    Public Sub fn_ValidarCargaMasivaReasPrimas()
        Try
            If dtImportaReasPrim.Rows.Count > 0 Then
                Dim Folio_Onbase As String = ""
                Dim Pagar_A As String = ""
                Dim Validacio As Boolean = True
                Dim Folio As String = ""
                Dim Exito As Boolean = True
                Dim Respuesta As String = ""
                ' Se valida los datos 
                For Each registros In dtImportaReasPrim.Rows
                    'Folio = registros("Folio_Onbase") 'Aqui Provamos si mando un folio correcto 

                    Folio_Onbase = Funciones.fn_EjecutaStr("select Folio_Onbase_Siniestro from Mis_expediente_op where Id_Etiqueta_Pago = 1 and id_Pagar_A is null and Id_Tipo_Doc <> 28 and Folio_Onbase_Siniestro = '" & registros("Folio_Onbase") & "'")
                    Pagar_A = registros("Pagar_A").ToString()
                    If Folio_Onbase = "" Then
                        Mensaje.MuestraMensaje(Master.Titulo, ".:Informacion inclompeta:. el siguiente Folio onbase no cumple con los filtros requeridos: " + registros("Folio_Onbase").ToString() + " Pagar A: " + registros("Pagar_A").ToString(), TipoMsg.Falla)
                        Validacio = False
                        Exit Sub
                    End If
                    If Pagar_A = "" Then
                        Mensaje.MuestraMensaje(Master.Titulo, ".:Informacion inclompeta:. el siguiente Folio onbase no cumple con los filtros requeridos: " + registros("Folio_Onbase").ToString() + " Pagar A: Vacio " + registros("Pagar_A").ToString(), TipoMsg.Falla)
                        Validacio = False
                        Exit Sub
                    End If
                    If Pagar_A < "7" And Pagar_A > "8" Then
                        Mensaje.MuestraMensaje(Master.Titulo, ".:Informacion inclompeta:. el siguiente Folio onbase no cumple con los filtros requeridos: " + registros("Folio_Onbase").ToString() + " Pagar A: Diferente a Tercero y asegurado" + registros("Pagar_A").ToString(), TipoMsg.Falla)
                        Validacio = False
                        Exit Sub
                    End If
                Next
                ' Se actualizan los datos 
                If Validacio = True Then
                    For Each registros In dtImportaReasPrim.Rows
                        Respuesta = Funciones.fn_EjecutaStr("update Mis_expediente_op set id_Pagar_A = '" & registros("Pagar_A") & "'" & " where Id_Etiqueta_Pago = 1 and id_Pagar_A is null and Id_Tipo_Doc <> 28 and Folio_Onbase_Siniestro = " & registros("Folio_Onbase"))
                        Mensaje.MuestraMensaje(Master.Titulo, ".:Folios Onbase Actualizados:. ", TipoMsg.Falla)
                    Next
                    fn_LimpiarCargaPagar_A()

                End If
            Else
                Mensaje.MuestraMensaje(Master.Titulo, "Debe importar el Layout para validar los registros", TipoMsg.Advertencia)
            End If
        Catch ex As Exception
            Mensaje.MuestraMensaje(Master.Titulo, ex.Message + Response.ToString(), TipoMsg.Falla)
            'Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "btn_ValidarReas_Click: " & ex.Message)
        End Try
    End Sub
    Private Sub btn_LimpiarReasPrim_Click(sender As Object, e As EventArgs) Handles btn_LimpiarReasPrim.Click
        fn_LimpiarCargaPagar_A()
    End Sub
    Public Sub fn_LimpiarCargaPagar_A()
        dtImportaReasPrim.Rows.Clear()
        LlenaGrid(gvd_CargaMasivaIngPrimas, Nothing)
    End Sub
    Public Shared Sub LlenaGrid(ByRef gvd_Control As GridView, ByRef dtDatos As DataTable)
        gvd_Control.DataSource = dtDatos
        gvd_Control.DataBind()
    End Sub
End Class
