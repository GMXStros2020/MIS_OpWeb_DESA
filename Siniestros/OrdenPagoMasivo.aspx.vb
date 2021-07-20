
Imports System.Data
Imports System.Web.Script.Services
Imports System.Web.Services

Imports System.IO
Imports Mensaje

Partial Class Siniestros_OrdenPagoMasivo
    Inherits System.Web.UI.Page


    Sub Page_Load(ByVal Sender As Object, ByVal e As EventArgs) Handles Me.Load

        If Not IsPostBack Then
            Me.txtFechaEstimadaPago.Text = FechaEstimPago()
            'txt_fecha_ini.Text = "01/01/2020"
            'txt_fecha_fin.Text = "15/11/2020"
            'txt_folio_onbase_Desde.Text = "19733"
            'txt_folio_onbase_Hasta.Text = "19733"
            'cmbPagarA.SelectedValue = 10
            'cmbSubsiniestro.SelectedValue = -1
        End If

        Cargar_combos() 'FJCP_10290_CC	INI   		

    End Sub




    Protected Sub btn_Revisar_Click(sender As Object, e As EventArgs) Handles btn_Revisar.Click

        Dim ws As New ws_Generales.GeneralesClient
        Dim server As String = ws.ObtieneParametro(3)
        Dim RptFilters As String
        RptFilters = "&NumLote=" & txt_NumLote.Text


        server = Replace(Replace(server, "@Reporte", "RevisionOrdenPagoMasiva"), "@Formato", "EXCEL")
        server = Replace(server, "ReportesGMX_DESA", "ReportesOPSiniestros_DESA")
        server = server & RptFilters
        Funciones.EjecutaFuncion("window.open('" & server & "');")





    End Sub


    Protected Sub btn_Aceptar_Click(sender As Object, e As EventArgs) Handles btn_Aceptar.Click



        Dim cadena As String
        Dim datos() As String
        cadena = HttpContext.Current.User.Identity.Name.ToString()
        datos = cadena.Split("|")



        Dim oDatos As DataSet
        Dim oTabla As DataTable
        Dim oParametros As New Dictionary(Of String, Object)


        oParametros.Add("Num_Lote", txt_NumLote.Text)
        oParametros.Add("salida", "2")


        oDatos = Funciones.ObtenerDatos("sp_recupera_lote_OP_Masivo", oParametros)
        oTabla = oDatos.Tables(0)



        For Each row As DataRow In oTabla.Rows

            oDatos = New DataSet

            oParametros = New Dictionary(Of String, Object)
            oParametros.Add("NumLote", txt_NumLote.Text)
            oParametros.Add("UsuarioSII", Master.cod_usuario.ToString())
            oParametros.Add("Folio_Onbase", row("Folio_Onbase").ToString())


            oDatos = Funciones.ObtenerDatos("usp_CrearSolicitudPago_stro_Masivo", oParametros)

        Next

        Response.Redirect("ResumenOPMasivo.aspx?Num_Lote=" & txt_NumLote.Text)

    End Sub


    Protected Sub btnLimpiar_Click(sender As Object, e As EventArgs) Handles btnLimpiar.Click
        Response.Redirect("OrdenPagoMasivo.aspx")

    End Sub

    Public Sub txt_TextChanged(sender As Object, e As EventArgs)


        If Convert.ToDateTime(Me.txtFechaEstimadaPago.Text) < Now.ToShortDateString Then
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", "No Puede ingresar una fecha menor al dia de hoy", TipoMsg.Advertencia)
            Me.txtFechaEstimadaPago.Text = Now.ToString("dd/MM/yyyy")
        End If




    End Sub

    Private Function FechaEstimPago() As String
        Dim oParametros As New Dictionary(Of String, Object)
        Dim oDatos As DataSet
        Dim dt As DataTable
        Dim result As String
        Try
            oParametros = New Dictionary(Of String, Object)







            oDatos = New DataSet

            oDatos = Funciones.ObtenerDatos("usp_Obtener_fecha_estimada_pago", oParametros)

            If Not oDatos Is Nothing AndAlso oDatos.Tables(0).Rows.Count > 0 Then
                result = oDatos.Tables(0).Rows(0).Item(0)
            Else
                result = ""
            End If


        Catch ex As Exception

            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("ObtenerTipoCambio error: {0}", ex.Message), TipoMsg.Falla)
        End Try

        FechaEstimPago = result
    End Function


    Private Sub btnAcepmTer_Click(sender As Object, e As EventArgs) Handles btnAcepmTer.Click
        Dim oDatos As DataSet
        Dim oTabla As DataTable
        Dim oParametros As New Dictionary(Of String, Object)
        Dim dtResult As New DataTable
        Dim Tercero As New Tercero()
        'ELIMNAR
        'drTipoPers.SelectedValue = "J"
        'txt_apPatmTer.Text = "SISTRAN TEST"
        'txt_fecNacmTer.Text = "04/04/1995"
        'txtRFCmTer.Text = "SIS950404KY7"
        If ValidaDatos() Then
            Tercero.sApellido1 = txt_apPatmTer.Text.Trim
            Tercero.sApellido2 = txt_apMatmTer.Text.Trim
            Tercero.sNombre = txt_nombresmTer.Text.Trim
            Tercero.sNit = txtRFCmTer.Text.Trim
            Tercero.vFecNacim = Funciones.FormatearFecha(txt_fecNacmTer.Text.Trim, Funciones.enumFormatoFecha.YYYYMMDD)
            Tercero.sTipoPersona = drTipoPers.SelectedValue
            If Tercero.RFC Then
                If drTipoPers.SelectedValue = "F" Then
                    oParametros.Add("txtApellido1", txt_apPatmTer.Text.Trim)
                    oParametros.Add("txtApellido2", txt_apMatmTer.Text.Trim)
                    oParametros.Add("txtNombre", txt_nombresmTer.Text.Trim)
                    oParametros.Add("codTipoDoc", 1)
                    oParametros.Add("nroDoc", txtRFCmTer.Text.Trim)
                    oParametros.Add("nroNit", txtRFCmTer.Text.Trim)
                    oParametros.Add("fecNac", Funciones.FormatearFecha(txt_fecNacmTer.Text.Trim, Funciones.enumFormatoFecha.YYYYMMDD))
                    oParametros.Add("txtSexo", drSexo.SelectedValue)
                    oParametros.Add("codEstCivil", 1)
                    oParametros.Add("codTipoPersona", drTipoPers.SelectedValue)
                    oParametros.Add("txtOrigen", "T")
                    oParametros.Add("codOcupacion", -1)
                    oParametros.Add("codUsuario", Master.cod_usuario)
                    oParametros.Add("nroEdad", hidEdadmTer.Value)
                    oParametros.Add("dGenerales", 1)
                Else
                    oParametros.Add("txtApellido1", txt_apPatmTer.Text.Trim)
                    oParametros.Add("codTipoDoc", 1)
                    oParametros.Add("nroDoc", txtRFCmTer.Text.Trim)
                    oParametros.Add("nroNit", txtRFCmTer.Text.Trim)
                    oParametros.Add("txtSexo", "F")
                    oParametros.Add("codEstCivil", 1)
                    oParametros.Add("codTipoPersona", drTipoPers.SelectedValue)
                    oParametros.Add("txtOrigen", "T")
                    oParametros.Add("codUsuario", Master.cod_usuario)
                    oParametros.Add("nroEdad", hidEdadmTer.Value)
                    oParametros.Add("dGenerales", 1)
                End If
                oDatos = Funciones.ObtenerDatos("usp_Alta_Upt_Tercero", oParametros)
                oTabla = oDatos.Tables(0)
                If Not oDatos Is Nothing AndAlso oDatos.Tables(0).Rows.Count > 0 Then
                    Dim codError As Integer
                    Dim msgError As String
                    Dim codTerceroNvo As Integer
                    codTerceroNvo = oTabla.Rows(0)("codTercero")
                    codError = oTabla.Rows(0)("snFlag")
                    msgError = oTabla.Rows(0)("msg_err").ToString()
                    If codError <> -1 Then
                        Funciones.CerrarModal("#RegistroTerceros")
                        Funciones.fn_Consulta("spS_CatalogosSIR @strCatalogo = 'BenTercero_stro', @Condicion = " + codTerceroNvo.ToString(), dtResult)
                        hidCodTercero.Value = dtResult.Rows(0)("Clave")
                        hidNomTercero.Value = dtResult.Rows(0)("Descripcion").ToString()
                        hidrfcTercero.Value = dtResult.Rows(0)("OcultaCampo1").ToString()
                        limpRegTerceros()

                        Funciones.EjecutaFuncion("selecTercero(" + ID_row.Value + ");")
                    Else
                        MuestraMensaje("Validación", "Error: " + msgError + ". No se pudo grabar el tercero.", TipoMsg.Advertencia)
                    End If
                End If
            End If
        End If
    End Sub

    Private Function ValidaDatos() As Boolean
        Dim msgError As String
        msgError = ""
        If drTipoPers.SelectedValue = "F" Then
            If Trim(txt_apPatmTer.Text) = "" Then msgError = msgError & IIf(msgError = "", "", ",") & "Apellido Paterno"
            If Trim(txt_apMatmTer.Text) = "" Then msgError = msgError & IIf(msgError = "", "", ",") & "Apellido Materno "
            If Trim(txt_nombresmTer.Text) = "" Then msgError = msgError & IIf(msgError = "", "", ",") & "Nombres "
            If Trim(txt_fecNacmTer.Text) = "" Then msgError = msgError & IIf(msgError = "", "", ",") & "Fecha Nacimiento "
            If Trim(txtRFCmTer.Text) = "" Then msgError = msgError & IIf(msgError = "", "", ",") & "RFC "
        Else
            If Trim(txt_apPatmTer.Text) = "" Then msgError = msgError & IIf(msgError = "", "", ",") & "Razón Social"
            If Trim(txt_fecNacmTer.Text) = "" Then msgError = msgError & IIf(msgError = "", "", ",") & "Fecha Nacimiento "
            If Trim(txtRFCmTer.Text) = "" Then msgError = msgError & IIf(msgError = "", "", ",") & "RFC "
        End If
        If msgError <> "" Then
            MuestraMensaje("Valida campos", "Los campos " & msgError & "no puedes estar vacíos", TipoMsg.Advertencia)
            Return False
        End If
        Return True
    End Function

    Private Sub limpRegTerceros()
        drTipoPers.SelectedValue = "F"
        txtRFCmTer.Text = ""
        txt_apPatmTer.Text = ""
        txt_apMatmTer.Text = ""
        txt_nombresmTer.Text = ""
        txt_fecNacmTer.Text = ""
    End Sub
    Private Sub Cargar_combos() 'FJCP_10290_CC	

        cmbTipoPago.Items.Clear()
        Dim dt As New DataTable
        Funciones.fn_Consulta("sp_Catalogos_OPMasivas 'MIS_Tipo_Pago','',''", dt)

        If cmbTipoPago.Items.Count = 0 Then
            cmbTipoPago.DataSource = dt
            cmbTipoPago.DataTextField = "Descripcion_Tipo_pago"
            cmbTipoPago.DataValueField = "Id_Tipo_pago"
            cmbTipoPago.DataBind()
        End If

        cmbTipoPagoPoT.Items.Clear()
        Dim dtt As New DataTable
        Funciones.fn_Consulta("sp_Catalogos_OPMasivas 'MIS_Pago_PoT','',''", dtt)

        If cmbTipoPagoPoT.Items.Count = 0 Then
            cmbTipoPagoPoT.DataSource = dtt
            cmbTipoPagoPoT.DataTextField = "Descripcion_Pago"
            cmbTipoPagoPoT.DataValueField = "Id_Pago"
            cmbTipoPagoPoT.DataBind()
        End If

        cmbTipoComprobante.Items.Clear()
        Dim dts As New DataTable
        Funciones.fn_Consulta("sp_Catalogos_OPMasivas 'COMPROBANTE','',''", dts)

        If cmbTipoComprobante.Items.Count = 0 Then

            cmbTipoComprobante.DataSource = dts
            cmbTipoComprobante.DataTextField = "Descripcion"
            cmbTipoComprobante.DataValueField = "CodigoComprobante"
            cmbTipoComprobante.DataBind()
        End If

    End Sub

    Private Sub btn_exportar_xls_Click(sender As Object, e As EventArgs) Handles btn_exportar_xls.Click
        Dim ws As New ws_Generales.GeneralesClient
        Dim server As String = ws.ObtieneParametro(3)
        Dim RptFilters As String

        Dim lote As String


        lote = hid_nLote.Value


        RptFilters = "&Num_Lote=" & lote.ToString()
        RptFilters = RptFilters & "&Accion=0"

        server = Replace(Replace(server, "@Reporte", "XLS_Varios_Multipago"), "@Formato", "EXCEL")
        server = Replace(server, "ReportesGMX_DESA", "ReportesOPSiniestros_DESA")
        server = server & RptFilters
        Funciones.EjecutaFuncion("window.open('" & server & "');")


    End Sub


End Class
