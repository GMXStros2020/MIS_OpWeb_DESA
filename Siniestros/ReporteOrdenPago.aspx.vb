Imports System.Data
Imports Mensaje

Partial Class Siniestros_ReporteOrdenPago
    Inherits System.Web.UI.Page

#Region "Eventos"
    Sub Page_Load(ByVal Sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            CargarCombos()
        End If
    End Sub

    Private Sub btnLimpiar_Click(sender As Object, e As EventArgs) Handles btnLimpiar.Click
        txt_fec_gen_desde.Text = ""
        txt_fec_gen_hasta.Text = ""
        CargarCombos()
        ddl_Pagar_a.SelectedValue = -1
    End Sub

    Private Sub btnReporte_Click(sender As Object, e As EventArgs) Handles btnReporte.Click
        If ValidaFiltros() Then
            GeneraReporte()
            CargarCombos()
            MuestraMensaje("Reporte ordenes de pago", "Reporte generado con éxito.", TipoMsg.Advertencia)
        End If
    End Sub
#End Region

#Region "Métodos"
    Private Sub CargarCombos()
        Dim oDatos As DataSet
        Dim dt As DataTable
        Dim oParametros As New Dictionary(Of String, Object)

        txt_fec_gen_desde.Text = ""
        txt_fec_gen_hasta.Text = ""

        oParametros.Add("strCatalogo", "statusOP")

        oDatos = Funciones.ObtenerDatos("sp_Catalogos_Repores_OPWEB", oParametros)

        ddl_Estatus.DataSource = oDatos.Tables(0)
        ddl_Estatus.DataValueField = "cod_estatus_op"
        ddl_Estatus.DataTextField = "descripcion"
        ddl_Estatus.DataBind()

        dt = New DataTable
        oParametros = New Dictionary(Of String, Object)
        oParametros.Add("strCatalogo", "PagarA")

        oDatos = Funciones.ObtenerDatos("sp_Catalogos_Repores_OPWEB", oParametros)

        ddl_Pagar_a.DataSource = oDatos.Tables(0)
        ddl_Pagar_a.DataValueField = "cod_abona"
        ddl_Pagar_a.DataTextField = "descripcion"
        ddl_Pagar_a.DataBind()

    End Sub

    Private Sub GeneraReporte()
        Dim ws As New ws_Generales.GeneralesClient
        Dim server As String = ws.ObtieneParametro(Cons.TargetReport)

        Dim RptFilters As String
        RptFilters = "&Fec_gene_ini=" & Funciones.FormatearFecha(txt_fec_gen_desde.Text, Funciones.enumFormatoFecha.YYYYMMDD)
        RptFilters = RptFilters & "&Fec_gene_fin=" & Funciones.FormatearFecha(txt_fec_gen_hasta.Text, Funciones.enumFormatoFecha.YYYYMMDD)
        RptFilters = RptFilters & "&cod_estatus_op=" & ddl_Estatus.SelectedValue.ToString()
        RptFilters = RptFilters & "&Pagar_a=" & ddl_Pagar_a.SelectedValue.ToString()

        server = Replace(Replace(server, "@Reporte", "Rpt_op_web"), "@Formato", "EXCEL")
        server = Replace(server, Cons.ReposSource, Cons.ReposReport)

        server = server & RptFilters
        Funciones.EjecutaFuncion("window.open('" & server & "');")
    End Sub
#End Region

#Region "Funciones"
    Private Function ValidaFiltros() As Boolean
        Dim fec_desde As Date
        Dim fec_hasta As Date

        ValidaFiltros = True

        If Trim(txt_fec_gen_desde.Text) = "" Or Trim(txt_fec_gen_hasta.Text) = "" Then
            If Trim(txt_fec_gen_hasta.Text) = "" Then
                MuestraMensaje("Validación", "Debe elegir un rango de fechas para la consulta", TipoMsg.Advertencia)
                ValidaFiltros = False
            Else
                MuestraMensaje("Validación", "Debe seleccionar fecha Genera desde", TipoMsg.Advertencia)
                ValidaFiltros = False
            End If
        Else
            If Trim(txt_fec_gen_hasta.Text) <> "" Then
                fec_desde = txt_fec_gen_desde.Text
                fec_hasta = txt_fec_gen_hasta.Text

                If fec_desde > fec_hasta Then
                    MuestraMensaje("Validación", "Fecha generación desde debe ser menor que fecha generación hasta", TipoMsg.Advertencia)
                    ValidaFiltros = False
                End If
            End If
        End If

        'If Trim(txt_fec_gen_desde.Text) <> "" And Trim(txt_fec_gen_hasta.Text) <> "" Then
        '    If Convert.ToDateTime(Me.txt_fec_gen_desde.Text) > Now.ToShortDateString Or Convert.ToDateTime(Me.txt_fec_gen_hasta.Text) > Now.ToShortDateString Then
        '        MuestraMensaje("Validación", "No Puede ingresar una fecha mayor al dia de hoy", TipoMsg.Advertencia)
        '        ValidaFiltros = False
        '    End If
        'End If

    End Function
#End Region
End Class
