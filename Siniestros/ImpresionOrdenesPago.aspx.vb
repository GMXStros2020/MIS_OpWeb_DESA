Imports System.Data
Imports System.Web.Script.Services
Imports System.Web.Services
Imports System.Data.SqlClient
Imports Mensaje
Imports Funciones
Imports System.IO
Imports System.Diagnostics
Imports System.Net

Partial Class Siniestros_ImpresionOrdenesPago
    Inherits System.Web.UI.Page
    Private Sub OrdenPago_Impresiones_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            If Not IsPostBack Then
                Master.Titulo = "Impresión Ordenes de Pago"
                Master.cod_modulo = Cons.ModuloRea
                Master.cod_submodulo = Cons.SubModOrdenPago

                Master.InformacionGeneral()
                Master.EvaluaPermisosModulo()

                Funciones.LlenaCatDDL(cmbMoneda, "Mon")
                cmbMoneda.SelectedValue = -1

                Dim DtUsuStro As New DataTable
                fn_Consulta("mis_UsuStroAdmin", DtUsuStro)
                If Not DtUsuStro Is Nothing Then
                    LlenaDDL(cmbElaborado, DtUsuStro,,, -1)

                End If

            End If
            ''EstadoDetalleOrden()
            ''ValidaUsrFiltros()
        Catch ex As Exception
            Funciones.fn_InsertaExcepcion(Master.cod_modulo, Master.cod_submodulo, Master.cod_usuario, "OrdenPago_Impresiones_Load: " & ex.Message)
        End Try
    End Sub

    Protected Sub btn_BuscarOP_Click(sender As Object, e As EventArgs) Handles btn_BuscarOP.Click
        Dim oDatos As DataSet
        Dim oTabla As DataTable
        Dim oParametros As New Dictionary(Of String, Object)
        Try
            If ValidaRadios() Then
                If ValidaFiltros() Then
                    If txt_NroOP.Text.Length <> 0 Then
                        oParametros.Add("nro_op", txt_NroOP.Text)
                    End If

                    If cmbMoneda.SelectedItem.Text <> ". . ." Then
                        oParametros.Add("Moneda", cmbMoneda.SelectedItem.Text)
                    End If

                    If txt_beneficiario.Text.Length <> 0 Then
                        oParametros.Add("Beneficiario", txt_beneficiario.Text)
                    End If

                    If txtFechaGeneracionDesde.Text.Length <> 0 Then
                        oParametros.Add("Fecha_Generacion_Desde", CDate(txtFechaGeneracionDesde.Text).ToString("yyyyMMdd"))
                    End If
                    If txtFechaGeneracionHasta.Text.Length <> 0 Then
                        oParametros.Add("Fecha_Generacion_Hasta", CDate(txtFechaGeneracionHasta.Text).ToString("yyyyMMdd"))
                    End If

                    If txtMontoDesde.Text.Length <> 0 Then
                        oParametros.Add("Monto_Desde", txtMontoDesde.Text)
                    End If
                    If txtMontoHasta.Text.Length <> 0 Then
                        oParametros.Add("Monto_Hasta", txtMontoHasta.Text)
                    End If

                    If cmbElaborado.SelectedItem.Text <> ". . ." Then
                        oParametros.Add("Elaborado_Por", cmbElaborado.SelectedItem.Value)
                    End If

                    If txtSiniestro.Text.Length <> 0 Then
                        oParametros.Add("Siniestro", txtSiniestro.Text)
                    End If

                    If TextFolio.Text.Length <> 0 Then
                        oParametros.Add("Folio", TextFolio.Text)
                    End If

                    If chk_Todas.Checked Then
                        oParametros.Add("Status", 1)
                    ElseIf chk_MisVig.Checked Then
                        oParametros.Add("Status", 2)
                    ElseIf chk_Rechazadas.Checked Then
                        oParametros.Add("Status", 3)
                    End If

                    oDatos = Funciones.ObtenerDatos("sp_consulta_ordenes_pago", oParametros)
                    oTabla = oDatos.Tables(0)

                    If oTabla.Rows.Count > 0 Then
                        grd.DataSource = oTabla
                        grd.DataBind()
                        Mostrar()
                    Else
                        MuestraMensaje("Validación", "No se encontro información", TipoMsg.Advertencia)
                        Ocultar()
                    End If
                End If
            Else
                MuestraMensaje("Validación", "Debe elegir un filtro de Ordenes de Pago", TipoMsg.Advertencia)
            End If

            BorrarAllArchivoTemporal()

        Catch ex As Exception

            Ocultar()
            MuestraMensaje("Exception", "Favor de validar la información de búsqueda.", TipoMsg.Falla)
        End Try
    End Sub

    Private Function ValidaRadios() As Boolean
        ValidaRadios = True
        If chk_Todas.Checked = False Then
            If chk_Rechazadas.Checked = False Then
                If chk_MisVig.Checked = False Then
                    ValidaRadios = False
                End If
            End If
        End If
    End Function

    Private Function ValidaFiltros() As Boolean
        ValidaFiltros = True

        If txtFechaGeneracionDesde.Text <> "" And txtFechaGeneracionHasta.Text = "" Then
            Mensaje.MuestraMensaje("Validacion filtro de Fecha", "La fecha de generación hasta se encuentra vacía.", TipoMsg.Advertencia)
            ValidaFiltros = False
            Ocultar()
        End If

        If txtFechaGeneracionHasta.Text <> "" And txtFechaGeneracionDesde.Text = "" Then
            Mensaje.MuestraMensaje("Validacion filtro de Fecha", "La fecha de generación desde se encuentra vacía.", TipoMsg.Advertencia)
            ValidaFiltros = False
            Ocultar()
        End If

        If txtFechaGeneracionHasta.Text <> "" And txtFechaGeneracionDesde.Text <> "" Then
            Dim anno_ini = txtFechaGeneracionDesde.Text.Substring(6, 4)
            Dim anno_fin = txtFechaGeneracionHasta.Text.Substring(6, 4)
            Dim mess_ini = txtFechaGeneracionDesde.Text.Substring(3, 2)
            Dim mess_fin = txtFechaGeneracionHasta.Text.Substring(3, 2)
            Dim dia_ini = txtFechaGeneracionDesde.Text.Substring(0, 2)
            Dim dia_fin = txtFechaGeneracionHasta.Text.Substring(0, 2)

            If (anno_ini > anno_fin) Then
                Mensaje.MuestraMensaje("Validacion filtro de Fecha", "La fecha de generación elegida no cuenta con un rango correcto.", TipoMsg.Advertencia)
                ValidaFiltros = False
                Ocultar()
            End If

            If (anno_ini = anno_fin And mess_ini > mess_fin) Then
                Mensaje.MuestraMensaje("Validacion filtro de Fecha", "La fecha de generación elegida no cuenta con un rango correcto.", TipoMsg.Advertencia)
                ValidaFiltros = False
                Ocultar()
            End If

            If (anno_ini = anno_fin And mess_ini = mess_fin And dia_ini > dia_fin) Then
                Mensaje.MuestraMensaje("Validacion filtro de Fecha", "La fecha de generación elegida no cuenta con un rango correcto.", TipoMsg.Advertencia)
                ValidaFiltros = False
                Ocultar()
            End If
        End If

        If txtMontoDesde.Text <> "" And txtMontoHasta.Text = "" Then
            Mensaje.MuestraMensaje("Validacion filtro de Monto", "El monto hasta se encuentra vacío.", TipoMsg.Advertencia)
            ValidaFiltros = False
            Ocultar()
        End If

        If txtMontoHasta.Text <> "" And txtMontoDesde.Text = "" Then
            Mensaje.MuestraMensaje("Validacion filtro de Monto", "El monto desde se encuentra vacío.", TipoMsg.Advertencia)
            ValidaFiltros = False
            Ocultar()
        End If

        If txtMontoHasta.Text <> "" And txtMontoDesde.Text <> "" Then
            Dim Montodesde = CInt(Int(txtMontoDesde.Text))
            Dim MontoHasta = CInt(Int(txtMontoHasta.Text))

            If Montodesde > MontoHasta Then
                Mensaje.MuestraMensaje("Validacion filtro de Monto", "El monto no cuenta con un rango correcto", TipoMsg.Advertencia)
                ValidaFiltros = False
                Ocultar()
            End If
        End If
    End Function

    Private Sub btn_Todas_Click(sender As Object, e As EventArgs) Handles btn_Todas.Click
        Try
            For Each row In grd.Rows
                Dim chkImp As CheckBox = TryCast(row.FindControl("chk_Print"), CheckBox)
                If chkImp.Enabled = True Then
                    chkImp.Checked = True
                End If
            Next
        Catch ex As Exception
            Mensaje.MuestraMensaje("Validacion", ex.Message, TipoMsg.Falla)
        End Try
    End Sub
    Private Sub btn_Ninguna_Click(sender As Object, e As EventArgs) Handles btn_Ninguna.Click
        Try
            For Each row In grd.Rows
                Dim chkImp As CheckBox = TryCast(row.FindControl("chk_Print"), CheckBox)
                If chkImp.Enabled = True Then
                    chkImp.Checked = False
                End If
            Next
        Catch ex As Exception
            Mensaje.MuestraMensaje("Validacion", ex.Message, TipoMsg.Falla)
        End Try
    End Sub

    Private Sub btn_Imprimir_Click(sender As Object, e As EventArgs) Handles btn_Imprimir.Click
        Dim dtSelec As DataTable
        Dim archivos() As String = Nothing
        Dim Index As Integer = 0
        Dim pdf = New reportePDF
        Dim strFoliosRep As String = ""
        Dim Arreglorutalocalfinal() As String = Nothing
        Dim rutacompleta As String
        Dim dt As New DataTable
        Dim rutaserver As String
        Dim Rutafinal As String

        dtSelec = obtenerSeleccionadosImp()
        Try
            If dtSelec.Rows.Count > 0 Then
                ReDim archivos(dtSelec.Rows.Count - 1)
                For Each row In dtSelec.Rows
                    archivos(Index) = row(0).ToString()
                    Index = Index + 1
                Next
            Else
                Index = 0
                MuestraMensaje("Validación", "No existen registros seleccionados para imprimir", TipoMsg.Advertencia)
                Exit Sub
            End If

            If archivos.Length > 0 AndAlso Index <> 0 Then
                pdf.Cod_usuario = Master.cod_usuario.ToString()
                pdf.Nro_ops = archivos
                pdf.GenerarRenombrar()
                Arreglorutalocalfinal = pdf.GenerarRenombrarlocal()

                Funciones.fn_Consulta("usp_obtDatosReporteOP 1", dt)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                rutaserver = dt.Rows(0)("rutaserverlocalhost").ToString
            End If

                For Each intem As String In Arreglorutalocalfinal
                    rutacompleta = rutaserver + intem
                    If Rutafinal <> "" Then
                        Rutafinal = Rutafinal + ";" + rutacompleta
                    Else
                        Rutafinal = rutacompleta
                    End If
                Next

                Funciones.EjecutaFuncion("fn_abrir_documento_Local('" & Rutafinal & "')")

            Else
                Index = 0
                MuestraMensaje("Validación", "Error al imprimir el PDF", TipoMsg.Falla)
            End If
            ' Response.Redirect("ImpresionOrdenesPago.aspx")

        Catch ex As Exception
            Mensaje.MuestraMensaje("Error en la impresión de la OP", ex.Message, TipoMsg.Falla)
        End Try
    End Sub

    Private Sub btn_Limpiar_Click(sender As Object, e As EventArgs) Handles btn_Limpiar.Click
        Response.Redirect("ImpresionOrdenesPago.aspx")
    End Sub
    Private Function obtenerSeleccionadosImp() As DataTable
        Try
            Dim dtSel As New DataTable
            Dim add As Boolean
            Dim indexTabla As Integer
            add = True
            indexTabla = 0

            If grd.Rows.Count = 0 Then
                MuestraMensaje("Validacion", "No existen registros en la tabla", TipoMsg.Advertencia)
                Return Nothing
                Exit Function
            End If

            dtSel.Columns.Add("NRO_OP")

            For index = 0 To grd.Rows.Count - 1
                Dim seleccionado As CheckBox
                seleccionado = grd.Rows(index).FindControl("chk_Print")
                If seleccionado.Checked Then
                    dtSel.Rows.Add()
                End If
            Next

            For Each row As GridViewRow In grd.Rows
                For i As Integer = 0 To row.Cells.Count - 1
                    Dim selecc As CheckBox
                    selecc = row.Cells(i).FindControl("chk_Print")

                    If selecc.Checked Then
                        If i = 1 Then
                            dtSel.Rows(indexTabla)(i - 1) = row.Cells(3).Text
                            add = True
                        End If
                    Else
                        add = False
                    End If
                Next
                If add Then
                    indexTabla = indexTabla + 1
                End If
            Next
            Return dtSel
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Private Sub btnEnviar_Click(sender As Object, e As EventArgs) Handles btnEnviar.Click
        Dim dtSelec As DataTable
        Dim archivos() As String = Nothing
        Dim Index As Integer = 0
        Dim pdf = New reportePDF
        Dim strFoliosRep As String = ""

        Dim Arreglorutalocalfinal As String
        'Dim rutacompleta As String
        Dim dt As New DataTable
        Dim rutaserver As String
        Dim Rutafinal As String


        dtSelec = obtenerSeleccionadosImp()
        Try
            If dtSelec.Rows.Count > 0 Then
                ReDim archivos(dtSelec.Rows.Count - 1)
                For Each row In dtSelec.Rows
                    archivos(Index) = row(0).ToString()
                    Index = Index + 1
                Next
            Else
                Index = 0
                MuestraMensaje("Validación", "No existen registros seleccionados para imprimir", TipoMsg.Advertencia)
                Exit Sub
            End If

            If archivos.Length > 0 AndAlso Index <> 0 Then
                pdf.Cod_usuario = Master.cod_usuario.ToString()
                pdf.Nro_ops = archivos
                pdf.Imprimir_todos()

                Arreglorutalocalfinal = pdf.Imprimir_todos_local()

                Funciones.fn_Consulta("usp_obtDatosReporteOP 1", dt)

                If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                    rutaserver = dt.Rows(0)("rutaserverlocalhost").ToString
                End If

                Rutafinal = rutaserver + Arreglorutalocalfinal

                Funciones.EjecutaFuncion("fn_abrir_documento_Local('" & Rutafinal & "')")

            Else
                Index = 0
                MuestraMensaje("Validación", "Error al imprimir los PDF", TipoMsg.Falla)
            End If

        Catch ex As Exception
            Mensaje.MuestraMensaje("Error en la impresión de las OP", ex.Message, TipoMsg.Falla)
        End Try
    End Sub

    Public Sub BorrarAllArchivoTemporal()
        Dim strFiles As List(Of String) = Directory.GetFiles("C:\inetpub\Impresion\", "*").ToList()

        For Each fichero As String In strFiles
            File.Delete(fichero)
        Next
    End Sub

    Private Sub Ocultar()
        btnEnviar.Visible = False
        btn_Imprimir.Visible = False
        btn_Ninguna.Visible = False
        btn_Todas.Visible = False
        grd.Visible = False
    End Sub
    Private Sub Mostrar()
        btnEnviar.Visible = True
        btn_Imprimir.Visible = True
        btn_Ninguna.Visible = True
        btn_Todas.Visible = True
        grd.Visible = True
    End Sub

    Private Sub txt_NroOP_TextChanged(sender As Object, e As EventArgs) Handles txt_NroOP.TextChanged

        txt_NroOP.Text = txt_NroOP.Text.Replace(" ", "")

        Dim Longitud = txt_NroOP.Text.Length
        Dim bandera As Boolean
        If IsNumeric(txt_NroOP.Text.Substring(Longitud - 1)) Then
            bandera = False
        Else
            bandera = True
        End If

        If bandera = True Then
            MuestraMensaje("Validación", "El filtro de Orden de Pago debe terminar en número.", TipoMsg.Falla)
        End If

    End Sub

End Class
