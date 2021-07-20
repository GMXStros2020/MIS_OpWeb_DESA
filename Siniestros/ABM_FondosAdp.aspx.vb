
Imports System.Data
Imports Mensaje

Partial Class Siniestros_ABM_FondosAdp
    Inherits System.Web.UI.Page
    Sub Page_Load(ByVal Sender As Object, ByVal e As EventArgs) Handles Me.Load

        If Not IsPostBack Then
            CargarCombo()
            CargarGrid()
        End If
    End Sub


    Protected Sub btnAgregar_Click(sender As Object, e As EventArgs) Handles btnAgregar.Click
        Dim oDatos As DataSet
        Dim oTabla As DataTable
        Dim iClave As Integer
        Dim oParametros As New Dictionary(Of String, Object)
        Try
            oParametros.Add("Accion", "1")
            oParametros.Add("cod_cpto", ddl_concepto_pago.SelectedValue)
            oParametros.Add("cod_clase_pago", ddl_clase_pago.SelectedValue)
            oParametros.Add("cod_origen_pago", cmbOrigen.SelectedValue)
            oParametros.Add("concepto_detalle", txtNombre.Text)


            oDatos = Funciones.ObtenerDatos("sp_grabar_conceptos_fondosAdp", oParametros)
            CargarGrid()
            CargarCombo()
            ddl_concepto_pago.Items.Clear()
            txtNombre.Text = "" 'FJCP_10290_CC

            Return

        Catch ex As Exception
            MuestraMensaje("Exception", "BuscarOP: " & ex.Message, TipoMsg.Falla)
            Return
        End Try
    End Sub



    Private Sub CargarGrid()
        Dim oDatos As DataSet
        Dim oTabla As DataTable
        Dim oParametros As New Dictionary(Of String, Object)
        Dim Num_Lote As String
        Dim Fondos As String


        oParametros.Add("Accion", 3)
        oDatos = Funciones.ObtenerDatos("sp_catalogos_FondosADP", oParametros)
        oTabla = oDatos.Tables(0)
        grd.DataSource = oTabla
        grd.DataBind()
    End Sub

    Private Sub CargarCombo()


        Dim oDatos As DataSet
        Dim oParametros As New Dictionary(Of String, Object)

        oParametros.Add("Accion", 6)
        'oParametros.Add("Folio_OnBase", "1")

        oDatos = Funciones.ObtenerDatos("sp_catalogos_FondosADP", oParametros)

        cmbOrigen.DataSource = oDatos.Tables(0)
        cmbOrigen.DataTextField = "DescripcionOrigenPago"
        cmbOrigen.DataValueField = "CodigoOrigenPago"
        cmbOrigen.DataBind()
        oDatos = New DataSet


        oParametros = New Dictionary(Of String, Object)
        oParametros.Add("Accion", 2)
        oDatos = Funciones.ObtenerDatos("[sp_catalogos_FondosADP]", oParametros)

        ddl_clase_pago.DataSource = oDatos.Tables(0)
        ddl_clase_pago.DataTextField = "txt_desc"
        ddl_clase_pago.DataValueField = "cod_clase_pago"
        ddl_clase_pago.DataBind()
    End Sub

    Protected Sub ddl_clase_pago_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddl_clase_pago.SelectedIndexChanged
        Dim oDatos As DataSet
        Dim oParametros As New Dictionary(Of String, Object)

        oParametros.Add("Accion", 1)
        oParametros.Add("cod_clase_pago", ddl_clase_pago.SelectedValue)

        oDatos = Funciones.ObtenerDatos("sp_catalogos_FondosADP", oParametros)

        ddl_concepto_pago.DataSource = oDatos.Tables(0)
        ddl_concepto_pago.DataTextField = "Descripcion"
        ddl_concepto_pago.DataValueField = "Concepto"
        ddl_concepto_pago.DataBind()
    End Sub

    Protected Sub grd_RowDeleting(sender As Object, e As GridViewDeleteEventArgs) Handles grd.RowDeleting

        Dim dt As New DataTable
        Dim oDatos As DataSet
        Dim oTabla As DataTable
        Dim iClave As Integer
        Dim oParametros As New Dictionary(Of String, Object)

        Try
            Dim row As GridViewRow = grd.Rows(e.RowIndex)
            Dim Concepto As String
            Dim Clase As String
            Concepto = row.Cells(0).Text
            Clase = row.Cells(5).Text

            oParametros.Add("Accion", "2")
            oParametros.Add("cod_cpto", Concepto)
            oParametros.Add("cod_clase_pago", Clase)

            oDatos = Funciones.ObtenerDatos("sp_grabar_conceptos_fondosAdp", oParametros)
            CargarGrid()


        Catch ex As Exception
            MuestraMensaje("Exception", ex.Message, TipoMsg.Falla)
        End Try
    End Sub

    Private Sub btnExportar_Click(sender As Object, e As EventArgs) Handles btnExportar.Click 'FJCP_10290_CC
        generaReporte()
    End Sub

    Private Sub generaReporte() 'FJCP_10290_CC
        Dim ws As New ws_Generales.GeneralesClient
        Dim server As String = ws.ObtieneParametro(3)
        Dim Random As New Random()
        Dim numero As Integer = Random.Next(1, 1000)

        Dim RptFilters As String
        RptFilters = "&numero=" & numero.ToString()
        server = Replace(Replace(server, "@Reporte", "Fondos_ADP"), "@Formato", "EXCEL")
        server = Replace(server, "ReportesGMX_DESA", "ReportesOPSiniestros_DESA")
        server = server & RptFilters
        Funciones.EjecutaFuncion("window.open('" & server & "');")

    End Sub


End Class
