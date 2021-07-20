
Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization
Imports Mensaje

Partial Class Siniestros_OrdenPago
    Inherits System.Web.UI.Page

#Region "Declaración de variables"

    Public Property oGrdOrden() As DataTable
        Get
            Return Session("grdOP")
        End Get
        Set(ByVal value As DataTable)
            Session("grdOP") = value
        End Set
    End Property

    Public Property oClavesPago() As DataTable
        Get
            Return Session("ClavesPago")
        End Get
        Set(ByVal value As DataTable)
            Session("ClavesPago") = value
        End Set
    End Property

    Public Property oOrigenesPago() As DataTable
        Get
            Return Session("OrigenesPago")
        End Get
        Set(ByVal value As DataTable)
            Session("OrigenesPago") = value
        End Set
    End Property

    Public Property oSeleccionActual() As DataTable
        Get
            Return Session("SeleccionActual")
        End Get
        Set(ByVal value As DataTable)
            Session("SeleccionActual") = value
        End Set
    End Property

    Public Property EdoOperacion() As Integer
        Get
            Return hid_Operacion.Value
        End Get
        Set(ByVal value As Integer)
            hid_Operacion.Value = value
        End Set
    End Property

    Public Property oCatalogoBancosT() As DataTable
        Get
            Return Session("BancosT")
        End Get
        Set(ByVal value As DataTable)
            Session("BancosT") = value
        End Set
    End Property

    Public Property oCatalogoTiposCuentaT() As DataTable
        Get
            Return Session("TiposCuentaT")
        End Get
        Set(ByVal value As DataTable)
            Session("TiposCuentaT") = value
        End Set
    End Property

    Public Property oCatalogoMonedasT() As DataTable
        Get
            Return Session("MonedasT")
        End Get
        Set(ByVal value As DataTable)
            Session("MonedasT") = value
        End Set
    End Property
    Public Enum eTipoUsuario
        Asegurado = 7
        Tercero = 8
        Proveedor = 10
    End Enum

#End Region

#Region "Eventos"
    Sub Page_Load(ByVal Sender As Object, ByVal e As EventArgs) Handles Me.Load

        If Not IsPostBack Then
            Master.Titulo = "OP Fondo/Aut.Varias"
            InicializarValores()
            CargarOrigenpago()
        End If

        If cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then
            Onbase.Style("display") = ""
            pnlProveedor.Style("display") = ""
            Facturas0.Style("display") = ""
            Facturas1.Style("display") = ""
            txtBeneficiario_stro.Enabled = False
            txtRFC.Enabled = False
            txtimporteTerAseg.Visible = False
            divauto_nes_varias.Style("display") = ""
            lblTitulo.Text = " Autorizaciones Varias "
            cmbConcepto.Items.Clear()
        Else
            pnlProveedor.Style("display") = "none"
            Facturas0.Style("display") = "none"
            Facturas1.Style("display") = "none"
            txtBeneficiario_stro.Enabled = True
            txtRFC.Enabled = True
            txtimporteTerAseg.Visible = True
            divauto_nes_varias.Style("display") = "none"
            lblTitulo.Text = " Fondos "
            cargatodoslosconceptos(2)
            CargaConceptocuenta()
        End If

    End Sub
    Public Sub CargarOrigenpago()
        Try
            Dim oDatos As DataSet
            oDatos = New DataSet
            Dim oParametros As New Dictionary(Of String, Object)
            oParametros.Add("Accion", "1")
            oDatos = Funciones.ObtenerDatos("MIS_sp_cir_op_stro_Catalogos_Fondos", oParametros)
            Me.cmbOrigendePago.Items.Clear()
            If (oDatos.Tables(0).Rows(0).Item("Mensaje") = "1") Then
                'With oDatos.Tables(0).Rows(0)
                '    Me.txtCodigoCuenta.Text = .Item("cod_cta_cble")
                '    Me.txtDescCuenta.Text = .Item("txt_denomin")
                'End With
                For Each fila In oDatos.Tables(0).Rows
                    Me.cmbOrigendePago.Items.Add(New ListItem(String.Format(fila.Item("cod_origen_pago")).ToUpper + " || " + fila.Item("desc_origen_pago"), fila.Item("cod_origen_pago")))
                Next

            Else
                Mensaje.MuestraMensaje("Debe ser numerico 1", oDatos.Tables(0).Rows(0).Item("Mensaje").ToString(), TipoMsg.Falla)
            End If
        Catch ex As Exception
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("grd_RowDataBound error: {0}", ex.Message), TipoMsg.Falla)
        End Try
    End Sub
    Public Sub EliminarFila(Elimi As Int16)
        Dim chkdelete As CheckBox

        Dim oDatos As DataTable

        Dim oFilasEliminadas As List(Of DataRow)

        Try

            oDatos = New DataTable

            oDatos = oGrdOrden

            oFilasEliminadas = New List(Of DataRow)

            For Each row In grd.Rows

                chkdelete = BuscarControlPorID(row, "eliminar")

                If chkdelete.Checked Then

                    grd.DeleteRow(row.RowIndex)
                    If oDatos.Rows.Count > 0 Then
                        oFilasEliminadas.Add(oDatos.Rows(row.RowIndex))
                    End If
                End If

            Next

            For Each oFila In oFilasEliminadas
                oDatos.Rows.Remove(oFila)
            Next

            oGrdOrden = oDatos

            grd.DataSource = oDatos
            grd.DataBind()

            If Elimi = 0 Then 'esto es para limpiar y recarcular los impuestos 
                CalcularTotales()

                ' cmbConceptoOP.Text = String.Empty

                If Not oDatos Is Nothing AndAlso oDatos.Rows.Count > 0 Then

                    For Each oFila In oDatos.Rows

                        'If cmbConceptoOP.Text.Trim = String.Empty Then
                        '    cmbConceptoOP.Text = String.Format("{0} {1}", cmbConceptoOP.Text.Trim, oFila("Siniestro"))
                        'Else
                        '    cmbConceptoOP.Text = String.Format("{0}, {1}", cmbConceptoOP.Text.Trim, oFila("Siniestro"))
                        'End If

                    Next

                    'cmbConceptoOP.Text = String.Format("{0} {1}", cmbConceptoOP.Text.Trim, oClavesPago.Select(String.Format("cod_clase_pago = '{0}'", oDatos.Rows(0)("ClasePago")))(0)("txt_desc"))

                Else
                    Me.cmbTipoUsuario.Enabled = True
                    Me.txtBeneficiario_stro.Enabled = True
                End If
            End If
        Catch ex As Exception
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("Quitar fila error: {0}", ex.Message), TipoMsg.Falla)
        End Try

    End Sub
    Public Sub btnQuitarFila_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnQuitarFila.Click
        EliminarFila(0)
    End Sub
    Public Sub grd_RowDeleting(ByVal sender As Object, ByVal e As GridViewDeleteEventArgs)

        'Dim oDatos As DataTable

        'Try

        '    'oDatos = New DataTable

        '    'oDatos = oGrdOrden

        '    'oDatos.Rows.RemoveAt(e.RowIndex)

        '    'oGrdOrden = oDatos

        '    'grd.DataSource = oDatos
        '    'grd.DataBind()

        '    'CalcularTotales()

        'Catch ex As Exception
        '    Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("grd_RowDeleting error: {0}", ex.Message), TipoMsg.Falla)
        'End Try

    End Sub
    Public Sub cmb_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)

        Dim cmb As DropDownList

        Dim row As GridViewRow

        Dim iFila As Integer

        Dim sElemento As String

        Dim oControlesExternos As List(Of String)

        Try

            cmb = sender
            row = Nothing

            oControlesExternos = New List(Of String)

            oControlesExternos.Add("cmbTipoPagoOP")
            oControlesExternos.Add("cmbTipoUsuario")

            If oControlesExternos.Contains(cmb.ID) Then 'Es un control no pertenece al grid
                sElemento = cmb.CssClass.Substring(cmb.CssClass.IndexOf(" "c) + 1)
                sElemento = Replace(sElemento, " Tablero", String.Empty)
            Else  'Es un control que pertenece al grid

                row = cmb.NamingContainer

                iFila = row.RowIndex

                sElemento = cmb.CssClass.Substring(cmb.CssClass.IndexOf(" "c) + 1)

            End If

            Select Case sElemento

                Case "tipoUsuario"
                    Me.txtBeneficiario.Text = String.Empty
                    Me.txtOnBase.Text = String.Empty
                    Me.txtSiniestro.Text = String.Empty
                    Me.txtCodigoCuenta.Text = String.Empty
                    Me.txtDescCuenta.Text = String.Empty
                    Me.txtRFC.Text = String.Empty
                    Me.txtCodigoBeneficiario_stro.Text = String.Empty
                    Me.txtidfactura.Text = String.Empty
                    Me.txtTotalFac.Text = String.Empty
                    Me.txtTotalAutorizacionFac.Text = String.Empty
                    Me.txtTotalImpuestosFac.Text = String.Empty
                    Me.txtTotalRetencionesFac.Text = String.Empty
                    Me.txtBeneficiario_stro.Text = String.Empty
                    Me.txtTotalAutorizacion.Text = String.Empty
                    'Me.txtTotalAutorizacionNacional.Text = String.Empty
                    Me.txtTotalImpuestos.Text = String.Empty
                    Me.txtTotalRetenciones.Text = String.Empty
                    Me.txtTotal.Text = String.Empty
                    Me.txtDescripcionOP.Text = String.Empty
                    Me.oSucursalT_stro.Value = String.Empty
                    Me.oBancoT_stro.Value = String.Empty
                    Me.oBeneficiarioT_stro.Value = String.Empty
                    Me.oCuentaBancariaT_stro.Value = String.Empty
                    Me.oMonedaT_stro.Value = String.Empty
                    Me.oTipoCuentaT_stro.Value = String.Empty
                    Me.oPlazaT_stro.Value = String.Empty
                    Me.oAbaT_stro.Value = String.Empty
                    Me.txtDescuentos.Text = String.Empty 'txt de facturas

                    'If Me.cmbConcepto.Items.Count > 0 Then
                    '    Me.cmbConcepto.Items.Clear()
                    'End If

                    'If Me.cmbconceptoProveedor.Items.Count > 0 Then
                    '    Me.cmbconceptoProveedor.Items.Clear()
                    'End If

                    If cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then
                        Me.cmbTipoPagoOP.SelectedValue = -1
                        Me.btnVerCuentas.Visible = True
                        Me.txtSiniestro.Enabled = True
                    Else
                        Me.cmbTipoPagoOP.SelectedValue = 0
                        Me.btnVerCuentas.Visible = False
                        Me.txtSiniestro.Enabled = True
                    End If

                Case "tipo_pago_OP"

                    Me.btnVerCuentas.Visible = IIf(cmbTipoPagoOP.SelectedValue = 0, False, True)

                Case "clase_pago"

                    If cmb.SelectedValue = "26" Then

                        If Not SiniestroAbierto(CLng(oGrdOrden.Rows(iFila)("Siniestro")), CInt(oGrdOrden.Rows(iFila)("Subsiniestro"))) Then
                            Mensaje.MuestraMensaje("Orden de pago de siniestros", String.Format("EL SINIESTRO {0} - {1} ESTA CERRADO O CANCELADO. SE TOMARA LA SIGUIENTE CLASE DE PAGO DISPONIBLE", oGrdOrden.Rows(iFila)("Siniestro"), oGrdOrden.Rows(iFila)("Subsiniestro")), TipoMsg.Advertencia)
                            oGrdOrden.Rows(iFila)("ClasePago") = oClavesPago.Rows(1).Item(0)
                            cmb.SelectedValue = oClavesPago.Rows(1).Item(0)
                        End If

                    End If

                    oGrdOrden.Rows(iFila)("ClasePago") = cmb.SelectedValue

                    'Selección automática en INDEMINIZACIONES
                    oGrdOrden.Rows(iFila)("ConceptoPago") = IIf(oGrdOrden.Rows(iFila)("ClasePago") = "26", "350", String.Empty)

                    CargarConceptosPago(row, iFila, cmb.SelectedValue)

                    'En caso de que el siniestro haya sido cancelado y se haya asignado la clase de pago
                    'de honorarios o gastos de siniestros se seleccionara el tipo de pago como final,
                    'de lo contrario se habilitara para poder cambiar el tipo de pago
                    If cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor AndAlso
                            (oGrdOrden.Rows(iFila)("ClasePago") = "27" OrElse oGrdOrden.Rows(iFila)("ClasePago") = "28") Then

                        cmb = New DropDownList
                        cmb = BuscarControlPorClase(row, "estandar-control tipo_pago")

                        If Not cmb Is Nothing Then
                            cmb.SelectedValue = "F"
                            cmb.Enabled = False
                        End If

                        oGrdOrden.Rows(iFila)("TipoPago") = 2

                    Else

                        cmb = New DropDownList
                        cmb = BuscarControlPorClase(row, "estandar-control tipo_pago")

                        If Not cmb Is Nothing Then
                            cmb.Enabled = True
                        End If

                    End If

                    CalcularTotales()

                Case "concepto_pago"

                    If (oGrdOrden.Rows(iFila)("ClasePago") = 26) Then

                        'Selección automática en INDEMINIZACIONES en caso de asegurados y terceros, 
                        'si es proveedor la que haya seleccionado
                        oGrdOrden.Rows(iFila)("ConceptoPago") = IIf(cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor, cmb.SelectedValue, "350")
                        'CargarConceptosPago(row, iFila, "26")

                    Else
                        oGrdOrden.Rows(iFila)("ConceptoPago") = cmb.SelectedValue
                    End If

                    CalcularTotales()

                Case "tipo_pago"

                    oGrdOrden.Rows(iFila)("TipoPago") = IIf(cmb.SelectedValue = "P", 1, 2)

            End Select

        Catch ex As Exception
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("cmb_SelectedIndexChanged error: {0}", ex.Message), TipoMsg.Falla)
        End Try

    End Sub
    Protected Sub grd_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)

        Dim oSelector As DropDownList

        Dim iIndex As Integer

        Try

            If e.Row.RowType = DataControlRowType.DataRow Then

                iIndex = e.Row.RowIndex

                'Clase de pago
                oSelector = New DropDownList
                oSelector = BuscarControlPorClase(e.Row, "estandar-control clase_pago")

                'oSelector.DataSource = oClavesPago
                'oSelector.DataTextField = "txt_desc"
                'oSelector.DataValueField = "cod_clase_pago"
                'oSelector.DataBind()

                'If oGrdOrden.Rows(iIndex)("ClasePago") = "6" Then


                '    'If Not SiniestroAbierto(CLng(Me.txtSiniestro.Text.Trim), CInt(Me.cmbSubsiniestro.SelectedValue)) Then
                '    '    Mensaje.MuestraMensaje("Orden de pago de siniestros", String.Format("EL SINIESTRO {0} - {1} ESTA CERRADO O CANCELADO. SE TOMARA LA SIGUIENTE OPCION DE CLASE DE PAGO", Me.txtSiniestro.Text.Trim, Me.cmbSubsiniestro.SelectedItem.Text), TipoMsg.Advertencia)
                '    '    oGrdOrden.Rows(iIndex)("ClasePago") = oClavesPago.Rows(1).Item(0)
                '    'End If

                'End If

                'oSelector.Items.FindByValue(oGrdOrden.Rows(iIndex)("ClasePago")).Selected = True

                'Concepto de pago
                'CargarConceptosPago(e.Row, iIndex, oGrdOrden.Rows(iIndex)("ClasePago"))

                'En caso de que el siniestro haya sido cancelado y se haya asignado la clase de pago
                'de honorarios o gastos de siniestros se seleccionara el tipo de pago como final,
                'de lo contrario se habilitara para poder cambiar el tipo de pago
                'If cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor AndAlso
                '        (oGrdOrden.Rows(iIndex)("ClasePago") = "27" OrElse oGrdOrden.Rows(iIndex)("ClasePago") = "28") Then

                '    oSelector = New DropDownList
                '    oSelector = BuscarControlPorClase(e.Row, "estandar-control tipo_pago")

                '    If Not oSelector Is Nothing Then
                '        oSelector.SelectedValue = "F"
                '        oSelector.Enabled = False
                '    End If

                '    oGrdOrden.Rows(iIndex)("TipoPago") = 2



                'Else

                '    oSelector = New DropDownList
                '    oSelector = BuscarControlPorClase(e.Row, "estandar-control tipo_pago")

                '    If Not oSelector Is Nothing Then
                '        oSelector.Enabled = True
                '    End If

                'End If

            End If

        Catch ex As Exception
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("grd_RowDataBound error: {0}", ex.Message), TipoMsg.Falla)
        End Try

    End Sub
    Public Sub cargatodoslosconceptos(Accion As Int16)
        Try
            Dim oDatos As DataSet
            oDatos = New DataSet
            Dim oParametros As New Dictionary(Of String, Object)
            oParametros.Add("Accion", Accion)
            oParametros.Add("cpto", 0)
            oDatos = Funciones.ObtenerDatos("MIS_sp_cir_op_stro_Consulta_Fondos", oParametros)
            cmbConcepto.Items.Clear()
            If (oDatos.Tables(0).Rows(0).Item("Mensaje") = "1") Then
                For Each fila In oDatos.Tables(0).Rows
                    Me.cmbConcepto.Items.Add(New ListItem(String.Format(fila.Item("cod_cpto")).ToUpper + " || " + fila.Item("txt_desc") + " || " + fila.Item("txt_denomin"), fila.Item("cod_cpto")))
                Next
            Else
                Mensaje.MuestraMensaje("Debe ser numerico 1", oDatos.Tables(0).Rows(0).Item("Mensaje").ToString(), TipoMsg.Falla)
            End If
        Catch ex As Exception
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("grd_RowDataBound error: {0}", ex.Message), TipoMsg.Falla)
        End Try
    End Sub
    Public Sub txt_textChangedConcepto(sender As Object, e As EventArgs)
        CargaConceptocuenta()
    End Sub
    Public Sub CargaConceptocuenta()
        Try
            Dim oDatos As DataSet
            oDatos = New DataSet
            Dim oParametros As New Dictionary(Of String, Object)
            oParametros.Add("Accion", 3)
                oParametros.Add("cpto", cmbConcepto.SelectedValue)
                oDatos = Funciones.ObtenerDatos("MIS_sp_cir_op_stro_Consulta_Fondos", oParametros)
            If (oDatos.Tables(0).Rows(0).Item("Mensaje") = "1") Then
                With oDatos.Tables(0).Rows(0)
                    Me.txtCodigoCuenta.Text = .Item("cod_cta_cble")
                    Me.txtDescCuenta.Text = .Item("txt_denomin")
                    Me.txtDescripcionOP.Text = Me.txtDescripcionOP.Text + " " + .Item("txt_denomin")
                End With
            Else
                Mensaje.MuestraMensaje("Debe ser numerico 1", oDatos.Tables(0).Rows(0).Item("Mensaje").ToString(), TipoMsg.Falla)
            End If
        Catch ex As Exception
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("grd_RowDataBound error: {0}", ex.Message), TipoMsg.Falla)
        End Try
    End Sub
    Public Sub txt_TextChanged(sender As Object, e As EventArgs)

        Dim oTxt As TextBox

        Dim sElemento As String = String.Empty

        Dim oDatos As DataSet

        Dim oDataTable As DataTable

        Dim oParametros As New Dictionary(Of String, Object)

        Dim oListaElementos As List(Of String)

        Try

            'If Not String.IsNullOrWhiteSpace(Me.txtFechaContable.Text.Trim) AndAlso IsDate(Me.txtFechaContable.Text.Trim) Then
            '    ObtenerTipoCambio()
            'Else
            '    Mensaje.MuestraMensaje("OrdenPagoSiniestros", "Ingrese una fecha válida", TipoMsg.Advertencia)
            '    Return
            'End If

            oDatos = New DataSet
            oDataTable = New DataTable

            oTxt = New TextBox

            oTxt = sender

            sElemento = oTxt.CssClass.Substring(oTxt.CssClass.IndexOf(" "c) + 1)
            sElemento = sElemento.Substring(0, sElemento.IndexOf(" "c))

            Select Case sElemento

                Case "onbase"
                Case "siniestro"
                Case "fechaContable"
                Case "importe"
                    If txtBeneficiario_stro.Text = "" Then
                        txtimporteTerAseg.Text = 0
                        Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("txt_TextChanged error: {0}", "Ingrese un tercero o asegurado"), TipoMsg.Falla)

                    Else
                        Me.txtTotalAutorizacion.Text = txtimporteTerAseg.Text 'importe de la poliza
                        Me.txtTotalImpuestos.Text = 0
                        Me.txtTotalRetenciones.Text = 0
                        Me.txtTotal.Text = txtimporteTerAseg.Text  'importe de la poliza

                        Me.iptxtTotalAutorizacion.Text = txtimporteTerAseg.Text 'importe de pago
                        Me.iptxtTotalImpuestos.Text = 0
                        Me.iptxtTotalRetenciones.Text = 0
                        Me.iptxtTotal.Text = txtimporteTerAseg.Text
                        Me.iptxtTotalNacional.Text = txtimporteTerAseg.Text
                        Me.txtBeneficiario.Text = txtBeneficiario_stro.Text
                    End If
            End Select


        Catch ex As Exception
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("txt_TextChanged error: {0}", ex.Message), TipoMsg.Falla)
        End Try

    End Sub
    Public Sub grid_TextChanged(sender As Object, e As EventArgs)

        Dim oTxt As TextBox
        Dim oFila As GridViewRow

        Dim iIndiceFila As Integer

        Dim sElemento As String = String.Empty

        Try

            oTxt = New TextBox

            oTxt = sender
            oFila = oTxt.NamingContainer
            iIndiceFila = oFila.RowIndex

            sElemento = oTxt.CssClass.Substring(oTxt.CssClass.IndexOf(" "c) + 1)

            Select Case sElemento

                Case "pago"

                    If IsNumeric(oTxt.Text.Trim) Then

                        If Not cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor AndAlso CDbl(oGrdOrden.Rows(iIndiceFila)("ImportePagos")) + CDbl(oTxt.Text.Trim) > CDbl(oGrdOrden.Rows(iIndiceFila)("Estimacion")) Then
                            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("Límite de estimación superado. {0} Estimación: {1} {2} Total de pagos: {3}",
                                                                                            Environment.NewLine,
                                                                                            CDbl(oGrdOrden.Rows(iIndiceFila)("Estimacion")),
                                                                                            Environment.NewLine,
                                                                                            CDbl(oGrdOrden.Rows(iIndiceFila)("ImportePagos"))), TipoMsg.Advertencia)
                        Else

                            If oGrdOrden.Columns("Pago").ReadOnly Then
                                oTxt.Text = oGrdOrden.Rows(iIndiceFila)("Pago")
                            Else

                                oGrdOrden.Rows(iIndiceFila)("Pago") = Math.Round(CDbl(oTxt.Text.Trim), 2)

                                CalcularTotales()

                            End If

                        End If

                    Else
                        oTxt.Text = IIf(IsDBNull(oGrdOrden.Rows(iIndiceFila)("Pago")), "", oGrdOrden.Rows(iIndiceFila)("Pago"))
                    End If
                Case "Descuentos"
                    If cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then
                        If txtDescuentos.Text = "" Then
                            txtDescuentos.Text = oTxt.Text
                            oGrdOrden.Rows(iIndiceFila)("Descuentos") = oTxt.Text
                            CalcularTotales()
                        Else
                            If Decimal.Parse(txtDescuentos.Text) >= 1 Then
                                txtDescuentos.Text = Decimal.Parse(txtDescuentos.Text) + Decimal.Parse(oTxt.Text)
                                oGrdOrden.Rows(iIndiceFila)("Descuentos") = oTxt.Text
                                CalcularTotales()
                            Else
                                txtDescuentos.Text = oTxt.Text
                                oGrdOrden.Rows(iIndiceFila)("Descuentos") = oTxt.Text
                                CalcularTotales()
                            End If
                        End If
                    Else 'esto es para la parte de terceros y asegurados que no se tiene contemplado los descuentos
                        txtDescuentos.Text = 0.0
                        oTxt.Text = txtDescuentos.Text
                        oGrdOrden.Rows(iIndiceFila)("Descuentos") = oTxt.Text
                    End If
                Case "deducibles"

                    If IsNumeric(oTxt.Text.Trim) Then
                        oGrdOrden.Rows(iIndiceFila)("Deducible") = CDbl(oTxt.Text.Trim)
                    Else
                        oTxt.Text = IIf(IsDBNull(oGrdOrden.Rows(iIndiceFila)("Deducible")), "", oGrdOrden.Rows(iIndiceFila)("Deducible"))
                    End If

                Case Else
                    Return

            End Select

        Catch ex As Exception
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("grid_TextChanged error: {0}", ex.Message), TipoMsg.Falla)
        End Try
    End Sub
    Public Sub AgregarFila(sender As Object, e As EventArgs) Handles btnAgregarFila.Click

        Dim rowIndex As Integer = 0

        Dim oTabla As DataTable

        Dim oFila As DataRow = Nothing

        Dim oFilaSeleccion() As DataRow

        Try
            oTabla = New DataTable()

            If Me.txtCodigoBeneficiario_stro.Text = String.Empty OrElse Me.txtBeneficiario_stro.Text.Trim = String.Empty Then
                Me.txtBeneficiario_stro.Text = String.Empty
                Me.txtCodigoBeneficiario_stro.Text = String.Empty
                Mensaje.MuestraMensaje("OrdenPagoSiniestros", "Codigo, Nombre o razón social no definido", TipoMsg.Advertencia)
                Return
            End If
            If Me.txtFechaComprobante.Text.Trim = String.Empty Then
                Mensaje.MuestraMensaje("OrdenPagoSiniestros", "No se ha indicando la fecha del comprobante", TipoMsg.Advertencia)
                Return
            End If
            'If cmbTipoUsuario.SelectedValue = eTipoUsuario.Tercero Then
            '    If Not ObtenerRFC(Me.txtCodigoBeneficiario_stro.Text) Then
            '        Mensaje.MuestraMensaje("OrdenPagoSiniestros", "Codigo Beneficiario Inexistente", TipoMsg.Advertencia)
            '        Return
            '    End If
            'End If

            'If cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then
            '    If Not ValidarFechaComprobante(Me.txtFechaComprobante.Text.Trim, CInt(Me.txtSiniestro.Text.Trim)) Then
            '        Mensaje.MuestraMensaje("OrdenPagoSiniestros", "Fecha de comprobante, menor a fecha ocurrencia stro(s)", TipoMsg.Advertencia)
            '        Return
            '    End If
            'End If

            If oGrdOrden IsNot Nothing Then

                Dim oRegistro = Nothing

                Select Case cmbTipoUsuario.SelectedValue

                    'Case eTipoUsuario.Asegurado, eTipoUsuario.Tercero
                    '    If String.IsNullOrWhiteSpace(txtSiniestro.Text) OrElse String.IsNullOrWhiteSpace(cmbConcepto.Text) Then
                    '        Mensaje.MuestraMensaje("OrdenPagoSiniestros", "Siniestro y/o concepto", TipoMsg.Falla)
                    '        Return
                    '    End If
                        'oRegistro = oGrdOrden.AsEnumerable().[Select](Function(x) New With {
                        '            Key .Siniestro = x.Field(Of String)("Siniestro"),
                        '            Key .RFC = x.Field(Of String)("RFC"),
                        '            Key .Factura = x.Field(Of String)("Factura")
                        '  }).Where(Function(s) s.Siniestro = txtSiniestro.Text.Trim() AndAlso s.RFC = txtRFC.Text.Trim())
                    Case eTipoUsuario.Proveedor
                        'If String.IsNullOrWhiteSpace(txtOnBase.Text) OrElse String.IsNullOrWhiteSpace(txtSiniestro.Text) OrElse Me.cmbconceptoProveedor.Items.Count = 0 Then
                        If String.IsNullOrWhiteSpace(txtOnBase.Text) OrElse String.IsNullOrWhiteSpace(txtSiniestro.Text) Then
                            Mensaje.MuestraMensaje("OrdenPagoSiniestros", "Folio Onbase, Numero siniestro o concepto del proveedor no valido", TipoMsg.Falla)
                            Return
                        End If
                        oRegistro = oGrdOrden.AsEnumerable().[Select](Function(x) New With {
                                    Key .Siniestro = x.Field(Of String)("Siniestro"),
                                    Key .RFC = x.Field(Of String)("RFC"),
                                    Key .Poliza = x.Field(Of String)("Poliza"),
                                    Key .Factura = x.Field(Of String)("Factura")
                          }).Where(Function(s) s.Siniestro = txtSiniestro.Text.Trim() AndAlso s.RFC = txtRFC.Text.Trim() AndAlso s.Factura = txtNumeroComprobante.Text.Trim()
                        ).FirstOrDefault()

                End Select

                If oRegistro Is Nothing Then

                    If oGrdOrden.Rows.Count > 0 Then
                        'Se valida que todos los registros correspondan a la misma póliza
                        If Not cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then
                            oFilaSeleccion = oGrdOrden.Select(String.Format("ConceptoPago = '{0}'", cmbConcepto.Text))
                        Else
                            'oFilaSeleccion = oGrdOrden.Select(String.Format("RFC = '{0}' AND Factura = '{1}'", txtRFC.Text, txtNumeroComprobante.Text))
                            oFilaSeleccion = oGrdOrden.Select(String.Format("RFC = '{0}' AND Cod_Pres = '{1}' AND OrigenPago = '{2}' AND Moneda = '{3}'", txtRFC.Text, txtcod_pres.Text, cmbOrigendePago.SelectedValue, cmbMonedaPago.SelectedItem.ToString()))
                        End If

                        If oFilaSeleccion.Length = 0 Then
                            Mensaje.MuestraMensaje("OrdenPagoSiniestros", "El RFC,Codigo proveedor, Origen de pago y/o Moneda no coincide para realisar un Multipago", TipoMsg.Advertencia)
                            Return
                        End If

                    End If

                    'Se obtienen datos secundarios
                    Select Case cmbTipoUsuario.SelectedValue
                        Case eTipoUsuario.Asegurado, eTipoUsuario.Tercero
                            oFilaSeleccion = oSeleccionActual.Select(String.Format("Poliza = {0} AND Factura = {1}", txtSiniestro.Text.Trim, cmbConcepto.Text.Trim()))
                        Case eTipoUsuario.Proveedor
                            'oFilaSeleccion = oSeleccionActual.Select(String.Format("nro_stro = {0} AND id_substro = {1} AND folio_GMX = {2}", txtSiniestro.Text.Trim, cmbconceptoProveedor.SelectedValue.ToString(), cmbconceptoProveedor.SelectedValue.ToString()))
                            'oFilaSeleccion = oSeleccionActual.Select(String.Format("RFC = {0}", txtSiniestro.Text.Trim()))
                            oFilaSeleccion = oGrdOrden.Select(String.Format("Poliza = '{0}' AND Factura = '{1}' ", txtCodigoCuenta.Text, txtNumeroComprobante.Text))
                        Case Else
                            oFilaSeleccion = Nothing
                    End Select

                    If Not oFilaSeleccion Is Nothing Then

                        oTabla = oGrdOrden

                        oFila = oTabla.NewRow()

                        oFila("Siniestro") = txtSiniestro.Text.Trim()
                        oFila("RFC") = txtRFC.Text.Trim()
                        oFila("Moneda") = cmbMonedaPago.SelectedItem.ToString()
                        oFila("ClasePago") = "615"
                        oFila("ConceptoPago") = cmbConcepto.SelectedValue
                        oFila("OrigenPago") = cmbOrigendePago.SelectedValue
                        oFila("Poliza") = txtCodigoCuenta.Text.Trim()
                        oFila("TipoMoneda") = cmbMonedaPago.SelectedValue 'oFilaSeleccion(0).Item("Moneda_poliza")
                        oFila("Deducible") = 0

                        oFila("IdSiniestro") = txtSiniestro.Text.Trim() 'oFilaSeleccion(0).Item("id_stro")
                        oFila("IdPersona") = txtCodigoBeneficiario_stro.Text 'oFilaSeleccion(0).Item("id_persona")
                        oFila("Cod_Pres") = txtcod_pres.Text 'oFilaSeleccion(0).Item("id_persona")

                        If Me.txtDescCuenta.Text = "NACIONAL" Then
                            Me.cmbMonedaPago.SelectedValue = 0
                        Else
                            Me.txtTipoCambio.Text = ObtenerTipoCambio().ToString
                        End If

                        Select Case cmbTipoUsuario.SelectedValue

                            Case eTipoUsuario.Asegurado, eTipoUsuario.Tercero
                                oFila("Factura") = String.Empty
                                oFila("FechaComprobante") = String.Empty
                                oFila("NumeroComprobante") = String.Empty
                                oFila("CodigoTercero") = oFilaSeleccion(0).Item("cod_tercero")
                                oFila("CodIndCob") = oFilaSeleccion(0).Item("cod_ind_cob")
                                oFila("SnCondusef") = oFilaSeleccion(0).Item("sn_condusef")
                                oFila("NumeroOficioCondusef") = oFilaSeleccion(0).Item("nro_oficio_condusef")
                                oFila("FechaOficioCondusef") = oFilaSeleccion(0).Item("fec_oficio_condusef")
                                oFila("NumeroCorrelaEstim") = oFilaSeleccion(0).Item("nro_correla_estim")
                                oFila("Estimacion") = oFilaSeleccion(0).Item("Estimacion")
                                oFila("ImportePagos") = oFilaSeleccion(0).Item("Total_Pago")
                                oFila("CodigoAsegurado") = oFilaSeleccion(0).Item("cod_aseg")
                                oFila("MonedaFactura") = 0
                            Case eTipoUsuario.Proveedor
                                oFila("FolioOnbase") = txtOnBase.Text
                                oFila("Factura") = txtNumeroComprobante.Text 'oFilaSeleccion(0).Item("folio_GMX")
                                oFila("CodigoTercero") = txtCodigoBeneficiario_stro.Text
                                oFila("CodIndCob") = 0
                                oFila("SnCondusef") = 0
                                oFila("NumeroOficioCondusef") = ""
                                oFila("FechaOficioCondusef") = ""
                                oFila("NumeroCorrelaEstim") = 0
                                'Importes e impuestos
                                oFila("Pago") = txtTotalFac.Text 'Math.Round(IIf(cmbMonedaPago.SelectedValue = 0, CDbl(oFilaSeleccion(0).Item("imp_subtotal")), CDbl(oFilaSeleccion(0).Item("imp_subtotal")) / CDbl(Me.txtTipoCambio.Text)), 2)
                                'Verificar si se queda
                                oFila("Impuestos") = txtTotalImpuestosFac.Text 'CDbl(oFilaSeleccion(0).Item("imp_impuestos"))
                                oFila("Retenciones") = txtTotalRetencionesFac.Text 'CDbl(oFilaSeleccion(0).Item("imp_retencion"))
                                'oFila("PagoSinIva") = CDbl(oFilaSeleccion(0).Item("PagoSinIva"))
                                'oFila("PagoConIva") = CDbl(oFilaSeleccion(0).Item("PagoConIva"))

                                oFila("FechaComprobante") = Me.txtFechaComprobante.Text.Trim
                                oFila("NumeroComprobante") = Me.txtNumeroComprobante.Text.Trim
                                ''oFila("CodigoAsegurado") = oFilaSeleccion(0).Item("cod_pres")
                                oFila("MonedaFactura") = cmbMonedaPago.SelectedValue ''oFilaSeleccion(0).Item("cod_moneda")
                                oFila("Subtotal") = txtTotalAutorizacionFac.Text ''oFilaSeleccion(0).Item("cod_moneda")
                        End Select

                        ''oFila("CodItem") = oFilaSeleccion(0).Item("cod_item")
                        ''oFila("CodigoRamo") = oFilaSeleccion(0).Item("Cod_ramo")
                        ''oFila("CodigoSubRamo") = oFilaSeleccion(0).Item("cod_subramo")
                        ''oFila("CodigoTipoStro") = oFilaSeleccion(0).Item("cod_tipo_stro")

                        oFila("TipoPago") = 1

                        oTabla.Columns("Pago").ReadOnly = IIf(cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor, True, False)

                        oTabla.Rows.Add(oFila)

                        oGrdOrden = oTabla

                        grd.DataSource = oTabla
                        grd.DataBind()

                        grd.Columns(2).Visible = IIf(cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor, True, False)

                        cmbTipoUsuario.Enabled = False

                        txtDescripcionOP.Text = ""

                        For Each oFila In oTabla.Rows

                            If txtDescripcionOP.Text.Trim = String.Empty Then
                                txtDescripcionOP.Text = String.Format("{0} {1}", txtDescripcionOP.Text.Trim, oFila("Siniestro"))
                            Else
                                txtDescripcionOP.Text = String.Format("{0}, {1}", txtDescripcionOP.Text.Trim, oFila("Siniestro"))
                            End If

                        Next

                        'txtDescripcionOP.Text = String.Format("{0} {1}", txtDescripcionOP.Text.Trim, oClavesPago.Select(String.Format("cod_clase_pago = '{0}'", oTabla.Rows(0)("ClasePago")))(0)("txt_desc"))
                        'txtDescripcionOP.Text = String.Format("{0} {1}", txtDescripcionOP.Text.Trim, String.Format("cod_clase_pago = '{0}'"), oTabla.Rows(0)("ClasePago")(0)("txt_desc"))
                        txtDescripcionOP.Text = txtDescripcionOP.Text + " " + txtSiniestro.Text '+ " " + cmbconceptoProveedor.SelectedItem.ToString()
                        If cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then
                            CalcularTotales()
                        End If

                        Me.txtBeneficiario.Text = Me.txtBeneficiario_stro.Text.Trim

                        Me.txtBeneficiario_stro.Enabled = False

                    Else
                        Mensaje.MuestraMensaje("OrdenPagoSiniestros", "No se pudo agregar la fila", TipoMsg.Advertencia)
                    End If

                Else
                    Mensaje.MuestraMensaje("OrdenPagoSiniestros", "Registro duplicado", TipoMsg.Advertencia)
                End If

            Else

                IniciaGrid()

                oTabla = oGrdOrden

                oFila = oTabla.NewRow()
                oFila("Factura") = IIf(cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor, txtOnBase.Text.Trim(), String.Empty)
                oFila("Siniestro") = txtSiniestro.Text.Trim()
                oFila("RFC") = cmbConcepto.Text.Trim()
                oFila("Moneda") = cmbMonedaPago.SelectedValue
                oFila("ClasePago") = "26"
                oFila("ConceptoPago") = "350"
                oFila("Poliza") = txtCodigoCuenta.Text.Trim()
                oFila("TipoMoneda") = IIf(txtDescCuenta.Text = "NACIONAL", 0, 1)

                oTabla.Rows.Add(oFila)

                oGrdOrden = oTabla

                grd.DataSource = oTabla
                grd.DataBind()

                grd.Columns(3).Visible = IIf(cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor, True, False)

                cmbTipoUsuario.Enabled = False

            End If

        Catch ex As Exception
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("AgregarFila error: {0}", ex.Message), TipoMsg.Falla)
        End Try

    End Sub
    Public Sub VerCuentasTransferencia() Handles btnVerCuentas.Click

        Dim oParametros As New Dictionary(Of String, Object)

        Dim oDatos As DataSet

        Dim bTieneDatosBancarios As Boolean

        Try

            'If grd.Rows.Count > 0 AndAlso cmbTipoPagoOP.SelectedValue = -1 Then

            oDatos = New DataSet

                oParametros = New Dictionary(Of String, Object)

                'oParametros.Add("Codigo", CInt(oGrdOrden.Rows(0).Item("IdPersona")))
                oParametros.Add("Codigo", txtCodigoBeneficiario_stro.Text)

                oDatos = Funciones.ObtenerDatos("usp_CargarDatosBancariosBeneficiario_stro", oParametros)

                If Not oDatos Is Nothing AndAlso oDatos.Tables(0).Rows.Count > 0 Then

                    With oDatos.Tables(0).Rows(0)

                        oBancoT_stro.Value = .Item("CodigoBanco")
                        oMonedaT_stro.Value = .Item("CodigoMoneda")
                        oTipoCuentaT_stro.Value = .Item("TipoCuenta")
                        oCuentaBancariaT_stro.Value = .Item("NumeroCuenta")
                        oBeneficiarioT_stro.Value = .Item("Beneficiario")

                        bTieneDatosBancarios = True

                    End With

                Else

                    If Me.cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then
                        Mensaje.MuestraMensaje("Cuentas bancarias", "No existen cuentas asociadas", TipoMsg.Falla)
                        Me.cmbTipoPagoOP.SelectedValue = 0
                        Me.btnVerCuentas.Visible = False
                        Return
                    End If

                    bTieneDatosBancarios = False

                End If

                oSucursalT_stro.Value = "CIUDAD DE MEXICO"
                oBeneficiarioT_stro.Value = IIf(oBeneficiarioT_stro.Value = String.Empty, Me.txtBeneficiario.Text.Trim, oBeneficiarioT_stro.Value)

                oParametros.Add("Banco", oBancoT_stro.Value)
                oParametros.Add("Sucursal", oSucursalT_stro.Value)
                oParametros.Add("Beneficiario", oBeneficiarioT_stro.Value)
                oParametros.Add("Moneda", oMonedaT_stro.Value)
                oParametros.Add("TipoCuenta", oTipoCuentaT_stro.Value)
                oParametros.Add("CuentaBancaria", oCuentaBancariaT_stro.Value)
                oParametros.Add("Plaza", oPlazaT_stro.Value)
                oParametros.Add("ABA", oAbaT_stro.Value)

                Master.MuestraTransferenciasBancariasSiniestros(IO.Path.GetFileName(Request.Url.AbsolutePath),
                                                                oCatalogoBancosT, oCatalogoTiposCuentaT, oCatalogoMonedasT,
                                                                oParametros, bTieneDatosBancarios)

           ' End If


        Catch ex As Exception
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("VerCuentasTransferencia error: {0}", ex.Message), TipoMsg.Falla)
        End Try

    End Sub
#End Region

#Region "Funciones"
    Private Function ValidarCondicionesImpuestos() As Boolean

        Dim oDatos As DataSet

        Dim oParametros As New Dictionary(Of String, Object)

        Dim iFila As Integer = 1

        Try

            ValidarCondicionesImpuestos = True

            For Each oFila In oGrdOrden.Rows

                oParametros = New Dictionary(Of String, Object)
                oDatos = New DataSet()

                'Se mandan estos parametros para obtener el id de la persona y el código de concepto
                oParametros.Add("CodigoProveedor", CInt(Me.txtCodigoBeneficiario_stro.Text))
                oParametros.Add("ClasePago", CInt(oFila("ClasePago")))

                oDatos = Funciones.ObtenerDatos("usp_ValidarCapturaCondicionesImpuestos_stro", oParametros)

                If oDatos Is Nothing OrElse oDatos.Tables(0).Rows.Count = 0 Then
                    Mensaje.MuestraMensaje("Condición de impuestos", String.Format("No se pueden calcular impuestos de la fila {0}, verifique clase y/o concepto de pago.", iFila), TipoMsg.Advertencia)
                    Return False
                End If

                iFila += 1

            Next

        Catch ex As Exception
            ValidarCondicionesImpuestos = False
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("ValidarCondicionesImpuestos error: {0}", ex.Message), TipoMsg.Falla)
        End Try

    End Function
    Private Function ObtenerRFC(ByVal sCodigoUsuario As String) As Boolean

        Dim oParametros As New Dictionary(Of String, Object)

        Dim oDatos As DataSet

        Try

            ObtenerRFC = False

            oParametros = New Dictionary(Of String, Object)

            oDatos = New DataSet

            oParametros.Add("CodigoUsuario", CInt(sCodigoUsuario))

            oDatos = Funciones.ObtenerDatos("usp_ObtenerRFC_stro", oParametros)

            If Not oDatos Is Nothing AndAlso oDatos.Tables(0).Rows.Count > 0 Then

                Me.txtRFC.Text = oDatos.Tables(0).Rows(0).Item("RFC").ToString.Trim

                ObtenerRFC = IIf(Me.txtRFC.Text.Trim = String.Empty, False, True)

            Else
                Me.txtRFC.Text = String.Empty
            End If

        Catch ex As Exception
            ObtenerRFC = False
            Me.txtRFC.Text = String.Empty
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("ObtenerRFC error: {0}", ex.Message), TipoMsg.Falla)
        End Try

    End Function
    Private Function GenerarXMLSolicitudPago(ByRef oSolicitudPago As StringBuilder) As Boolean

        Dim iNumeroCorrelaPagos As Integer = 1

        Try

            GenerarXMLSolicitudPago = False

            If ValidarDatos() Then

                oSolicitudPago.AppendLine("<SolicitudPago>")
                oSolicitudPago.AppendFormat("<UsuarioSII>{0}</UsuarioSII>", IIf(Master.cod_usuario = String.Empty, "JJIMENEZ", Master.cod_usuario))
                oSolicitudPago.AppendFormat("<NumeroSiniestro>{0}</NumeroSiniestro>", txtSiniestro.Text.Trim)
                oSolicitudPago.AppendFormat("<TotalPagoMoneda>{0}</TotalPagoMoneda>", CDbl(txtTotalAutorizacion.Text))
                oSolicitudPago.AppendFormat("<TotalPagoNacional>{0}</TotalPagoNacional>", IIf(cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor, CDbl(txtTotalNacional.Text), CDbl(txtTotalAutorizacion.Text)))
                oSolicitudPago.AppendFormat("<TotalIVA>{0}</TotalIVA>", CDbl(txtTotalImpuestos.Text))
                oSolicitudPago.AppendFormat("<TotalPago>{0}</TotalPago>", CDbl(txtTotal.Text))
                oSolicitudPago.AppendFormat("<VariasFacturas>{0}</VariasFacturas>", IIf(chkVariasFacturas.Checked, "Y", "N"))

                Select Case cmbTipoUsuario.SelectedValue
                    Case eTipoUsuario.Asegurado    'Asegurado
                        oSolicitudPago.AppendFormat("<TipoUsuario>{0}</TipoUsuario>", 7)
                        oSolicitudPago.AppendFormat("<TipoComprobante>{0}</TipoComprobante>", String.Empty)
                        oSolicitudPago.AppendFormat("<NumeroComprobante>{0}</NumeroComprobante>", String.Empty)
                        oSolicitudPago.AppendFormat("<FechaComprobante>{0}</FechaComprobante>", String.Empty)
                    Case eTipoUsuario.Tercero   'Tercero
                        oSolicitudPago.AppendFormat("<TipoUsuario>{0}</TipoUsuario>", 8)
                        oSolicitudPago.AppendFormat("<TipoComprobante>{0}</TipoComprobante>", String.Empty)
                        oSolicitudPago.AppendFormat("<NumeroComprobante>{0}</NumeroComprobante>", String.Empty)
                        oSolicitudPago.AppendFormat("<FechaComprobante>{0}</FechaComprobante>", String.Empty)
                    Case eTipoUsuario.Proveedor    'Proveedor
                        oSolicitudPago.AppendFormat("<TipoUsuario>{0}</TipoUsuario>", 10)
                        'oSolicitudPago.AppendFormat("<TipoComprobante>{0}</TipoComprobante>", cmbTipoComprobante.SelectedValue)
                        oSolicitudPago.AppendFormat("<NumeroComprobante>{0}</NumeroComprobante>", CInt(oGrdOrden.Rows(0).Item("NumeroComprobante")))
                        oSolicitudPago.AppendFormat("<FechaComprobante>{0}</FechaComprobante>", Convert.ToDateTime(oGrdOrden.Rows(0).Item("FechaCOmprobante")).ToString("yyyyMMdd"))
                End Select

                oSolicitudPago.AppendFormat("<IdPersona>{0}</IdPersona>", CInt(oGrdOrden.Rows(0).Item("IdPersona")))
                oSolicitudPago.AppendFormat("<CodigoAsegurado>{0}</CodigoAsegurado>", Me.txtCodigoBeneficiario_stro.Text.Trim)
                oSolicitudPago.AppendFormat("<CodigoSucursal>{0}</CodigoSucursal>", CInt(cmbSucursal.SelectedValue))
                oSolicitudPago.AppendFormat("<MonedaPago>{0}</MonedaPago>", CInt(cmbMonedaPago.SelectedValue))
                oSolicitudPago.AppendFormat("<Descripcion>{0}</Descripcion>", "Pruebas JJIMENEZ")
                oSolicitudPago.AppendFormat("<TipoMovimiento>3</TipoMovimiento>")
                oSolicitudPago.AppendFormat("<NombreRazon>{0}</NombreRazon>", Replace(Me.txtBeneficiario_stro.Text.Trim, "&", "&amp;"))

                Select Case cmbTipoPagoOP.SelectedValue
                    Case "C"
                        oSolicitudPago.AppendFormat("<TipoPago>{0}</TipoPago>", 1)
                    Case "T"
                        oSolicitudPago.AppendFormat("<TipoPago>{0}</TipoPago>", 2)
                End Select

                oSolicitudPago.AppendFormat("<CodigoRamo>{0}</CodigoRamo>", CInt(oGrdOrden.Rows(0).Item("CodigoRamo")))
                oSolicitudPago.AppendFormat("<CodigoSubRamo>{0}</CodigoSubRamo>", CInt(oGrdOrden.Rows(0).Item("CodigoSubRamo")))
                oSolicitudPago.AppendFormat("<Moneda>{0}</Moneda>", CInt(oGrdOrden.Rows(0).Item("TipoMoneda")))

                oSolicitudPago.AppendFormat("<TipoCambio>{0}</TipoCambio>", CDbl(txtTipoCambio.Text))
                oSolicitudPago.AppendFormat("<RFC>{0}</RFC>", txtRFC.Text.Trim)
                oSolicitudPago.AppendFormat("<CodigoOrigenPago>{0}</CodigoOrigenPago>", CInt(cmbOrigendePago.SelectedValue))
                oSolicitudPago.AppendFormat("<Observaciones>{0}</Observaciones>", Me.txtDescripcionOP.Text.Trim)
                oSolicitudPago.AppendFormat("<FechaIngreso>{0}</FechaIngreso>", Convert.ToDateTime(txtFechaContable.Text.Trim).ToString("yyyyMMdd"))

                For Each oFila In oGrdOrden.Rows

                    oSolicitudPago.AppendLine("<Detalle>")
                    oSolicitudPago.AppendFormat("<IdSiniestro>{0}</IdSiniestro>", CInt(oFila.Item("IdSiniestro")))
                    oSolicitudPago.AppendFormat("<Subsiniestro>{0}</Subsiniestro>", CInt(oFila.Item("Subsiniestro")))
                    oSolicitudPago.AppendFormat("<CodigoTercero>{0}</CodigoTercero>", CInt(oFila.Item("CodigoTercero")))
                    oSolicitudPago.AppendFormat("<ClasePago>{0}</ClasePago>", CInt(oFila.Item("ClasePago")))
                    oSolicitudPago.AppendFormat("<ConceptoPago>{0}</ConceptoPago>", CInt(oFila.Item("ConceptoPago")))

                    If cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then
                        oSolicitudPago.AppendFormat("<Pago>{0}</Pago>", CDbl(oFila.Item("Pago")))
                    Else
                        oSolicitudPago.AppendFormat("<Pago>{0}</Pago>", CDbl(oFila.Item("Pago")))
                        'oSolicitudPago.AppendFormat("<PagoFacturado>{0}</PagoFacturado>", CDbl(oFila.Item("Pago")))
                    End If

                    oSolicitudPago.AppendFormat("<CodItem>{0}</CodItem>", CInt(oFila.Item("CodItem")))
                    oSolicitudPago.AppendFormat("<CodIndCob>{0}</CodIndCob>", CInt(oFila.Item("CodIndCob")))
                    oSolicitudPago.AppendFormat("<Deducible>0</Deducible>", IIf(IsDBNull(oFila.Item("Deducible")), 0, CDbl(oFila.Item("Deducible"))))
                    oSolicitudPago.AppendFormat("<NumeroCorrelaEstim>{0}</NumeroCorrelaEstim>", CInt(oFila.Item("NumeroCorrelaEstim")))
                    oSolicitudPago.AppendFormat("<NumeroCorrelaPagos>{0}</NumeroCorrelaPagos>", iNumeroCorrelaPagos)
                    oSolicitudPago.AppendFormat("<SnCondusef>{0}</SnCondusef>", oFila.Item("SnCondusef"))
                    oSolicitudPago.AppendFormat("<NumeroOficioCondusef>{0}</NumeroOficioCondusef>", oFila.Item("NumeroOficioCondusef"))
                    oSolicitudPago.AppendFormat("<FechaOficioCondusef>{0}</FechaOficioCondusef>", oFila.Item("FechaOficioCondusef"))
                    oSolicitudPago.AppendFormat("<PagoNacional>{0}</PagoNacional>", IIf(cmbMonedaPago.SelectedValue = 0, CDbl(oFila.Item("Pago")), CDbl(oFila.Item("Pago")) * CDbl(txtTipoCambio.Text)))
                    oSolicitudPago.AppendFormat("<CodigoTipoStro>{0}</CodigoTipoStro>", CInt(oFila.Item("CodigoTipoStro")))
                    oSolicitudPago.AppendFormat("<TipoPagoDetalle>{0}</TipoPagoDetalle>", CInt(oFila.Item("TipoPago")))

                    oSolicitudPago.AppendLine("</Detalle>")

                    iNumeroCorrelaPagos += 1

                Next
                'Campos para transferencia
                If cmbTipoPagoOP.SelectedValue = "T" Then
                    oSolicitudPago.AppendFormat("<NumeroCuentaTransferencia>{0}</NumeroCuentaTransferencia>", oCuentaBancariaT_stro.Value)
                    oSolicitudPago.AppendFormat("<CodigoBancoTransferencia>{0}</CodigoBancoTransferencia>", IIf(oBancoT_stro.Value = String.Empty, String.Empty, CInt(oBancoT_stro.Value)))
                Else
                    oSolicitudPago.AppendLine("<NumeroCuentaTransferencia></NumeroCuentaTransferencia>")
                    oSolicitudPago.AppendLine("<CodigoBancoTransferencia></CodigoBancoTransferencia>")
                End If

                oSolicitudPago.AppendLine("</SolicitudPago>")

                GenerarXMLSolicitudPago = True

            End If

        Catch ex As Exception
            GenerarXMLSolicitudPago = False
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("GenerarXMLSolicitudPago error: {0}", ex.Message), TipoMsg.Falla)
        End Try

    End Function
    Private Function GenerarXMLImpuestos(ByRef oImpuestos As StringBuilder) As Boolean

        Dim oDatos As DataSet

        Dim iNumeroCorrelaPagos As Integer = 1

        Dim iMonedaPoliza As Integer

        Try

            GenerarXMLImpuestos = False

            iMonedaPoliza = CInt(oGrdOrden.Rows(0).Item("TipoMoneda"))

            oImpuestos.AppendLine("<Impuestos>")

            For Each oFila In oGrdOrden.Rows

                oDatos = New DataSet

                ObtenerDetalleImpuestos(oDatos, CInt(Me.txtCodigoBeneficiario_stro.Text), CInt(oFila("ClasePago")), CInt(oFila("ConceptoPago")), CInt(oFila("IdSiniestro")), CDbl(oFila("Pago")))

                If oDatos Is Nothing AndAlso oDatos.Tables(0).Rows.Count = 0 Then
                    Throw New Exception("Error al generar detalle de impuestos.")
                End If

                For Each oDetalle As DataRow In oDatos.Tables(0).Rows

                    oImpuestos.AppendLine("<Detalle>")

                    'Basado en campos de la tabla stro_op_p_impuesto_g_c
                    'El id_stro_op y cod_tercero no se mandan porque sera calculado en el sp que crea la orden de pago
                    oImpuestos.AppendFormat("<IdStro>{0}</IdStro>", CInt(oFila("IdSiniestro")))
                    oImpuestos.AppendFormat("<CodItem>{0}</CodItem>", CInt(oFila.Item("CodItem")))
                    oImpuestos.AppendFormat("<CodIndCob>{0}</CodIndCob>", CInt(oFila.Item("CodIndCob")))
                    oImpuestos.AppendFormat("<NumeroCorrelaPagos>{0}</NumeroCorrelaPagos>", iNumeroCorrelaPagos)
                    oImpuestos.AppendFormat("<CodigoConcepto>{0}</CodigoConcepto>", CInt(oDetalle("CodigoConcepto")))
                    oImpuestos.AppendFormat("<CodigoImpuesto>{0}</CodigoImpuesto>", CInt(oDetalle("CodigoImpuesto")))
                    oImpuestos.AppendFormat("<CodigoGrupo>{0}</CodigoGrupo>", CInt(oDetalle("CodigoGrupo")))
                    oImpuestos.AppendFormat("<CodigoCondicion>{0}</CodigoCondicion>", CInt(oDetalle("CodigoCondicion")))

                    'Si la moneda de la póliza son dolares se calculara con el tipo de cambio
                    oImpuestos.AppendFormat("<PjeImpuesto>{0}</PjeImpuesto>", IIf(iMonedaPoliza = 0, CDbl(oDetalle("PjeImpuesto")), Math.Round(CDbl(oDetalle("PjeImpuesto")) / CDbl(Me.txtTipoCambio.Text), 2)))
                    oImpuestos.AppendFormat("<Base>{0}</Base>", IIf(iMonedaPoliza = 0, CDbl(oFila("Pago")), Math.Round(CDbl(oFila("Pago")) / CDbl(Me.txtTipoCambio.Text), 2)))
                    oImpuestos.AppendFormat("<ImporteNoGravado>{0}</ImporteNoGravado>", IIf(iMonedaPoliza = 0, CDbl(oDetalle("ImporteNoGravado")), Math.Round(CDbl(oDetalle("ImporteNoGravado")) / CDbl(Me.txtTipoCambio.Text), 2)))
                    oImpuestos.AppendFormat("<ImporteImpuesto>{0}</ImporteImpuesto>", IIf(iMonedaPoliza = 0, CDbl(oDetalle("ImporteImpuesto")), Math.Round(CDbl(oDetalle("ImporteImpuesto")) / CDbl(Me.txtTipoCambio.Text), 2)))
                    oImpuestos.AppendFormat("<PjeRetencion>{0}</PjeRetencion>", IIf(iMonedaPoliza = 0, CDbl(oDetalle("PjeRetencion")), Math.Round(CDbl(oDetalle("PjeRetencion")) / CDbl(Me.txtTipoCambio.Text), 2)))
                    oImpuestos.AppendFormat("<ImporteRetencion>{0}</ImporteRetencion>", IIf(iMonedaPoliza = 0, CDbl(oDetalle("ImporteRetencion")), Math.Round(CDbl(oDetalle("ImporteRetencion")) / CDbl(Me.txtTipoCambio.Text), 2)))

                    oImpuestos.AppendFormat("<CodigoTratamiento>{0}</CodigoTratamiento>", CInt(oDetalle("CodigoTratamiento")))
                    oImpuestos.AppendFormat("<Subsiniestro>{0}</Subsiniestro>", CInt(oFila.Item("Subsiniestro")))

                    oImpuestos.AppendLine("</Detalle>")

                Next

                iNumeroCorrelaPagos += 1

            Next

            oImpuestos.AppendLine("</Impuestos>")

            GenerarXMLImpuestos = True

        Catch ex As Exception
            GenerarXMLImpuestos = False
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("GenerarXMLImpuestos error: {0}", ex.Message), TipoMsg.Falla)
        End Try

    End Function
    Private Function ValidarDatos() As Boolean

        Try

            ValidarDatos = False
            If cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then

                If Me.txtOnBase.Text.Trim = String.Empty Then
                    Throw New Exception("Folio Onbase no definido")
                End If

            End If
            If Me.txtSiniestro.Text.Trim = String.Empty OrElse String.IsNullOrWhiteSpace(cmbConcepto.Text) Then
                Throw New Exception("Número de siniestro no definido")
            End If
            If Me.cmbConcepto.Text.Trim = String.Empty OrElse String.IsNullOrWhiteSpace(cmbConcepto.Text) Then
                Throw New Exception("Concepto no definido")
            End If
            If Me.txtCodigoBeneficiario_stro.Text.Trim = String.Empty OrElse Me.txtBeneficiario_stro.Text = String.Empty Then
                Throw New Exception("Nombre o razón social no definido")
            End If
            If Me.iptxtTotal.Text.Trim = String.Empty OrElse String.IsNullOrWhiteSpace(cmbConcepto.Text) Then
                Throw New Exception("Importe no definido")
            End If
            'If oGrdOrden Is Nothing OrElse oGrdOrden.Rows.Count = 0 Then
            '    Throw New Exception("Error en lectura de datos")
            'End If

            'If grd Is Nothing OrElse grd.Rows.Count = 0 Then
            '    Throw New Exception("Error en lectura de registros")
            'End If

            'If Not grd.Rows.Count = oGrdOrden.Rows.Count Then
            '    Throw New Exception("Número de registros desigual.")
            'End If

            'If cmbOrigendePago.Items.Count = 0 Then
            '    Throw New Exception("origen para la orden de pago no definido")
            'End If

            If cmbTipoPagoOP.SelectedValue = "T" Then

                If cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor AndAlso
                    (String.IsNullOrWhiteSpace(oBancoT_stro.Value) OrElse String.IsNullOrWhiteSpace(oMonedaT_stro.Value) _
                    OrElse String.IsNullOrWhiteSpace(oTipoCuentaT_stro.Value) OrElse String.IsNullOrWhiteSpace(oCuentaBancariaT_stro.Value) _
                    OrElse String.IsNullOrWhiteSpace(oBeneficiarioT_stro.Value) OrElse String.IsNullOrWhiteSpace(oSucursalT_stro.Value) _
                    OrElse String.IsNullOrWhiteSpace(oBeneficiarioT_stro.Value)) Then

                    ObtenerDatosTransferenciaProveedor()

                End If

                If oBancoT_stro.Value.Trim = String.Empty Then
                    Throw New Exception("Banco no definido en cuenta de transferencia.")
                End If

                If oSucursalT_stro.Value.Trim = String.Empty Then
                    Throw New Exception("Sucursal no definida en cuenta de transferencia.")
                End If

                If oBeneficiarioT_stro.Value.Trim = String.Empty Then
                    Throw New Exception("Beneficiario no definido en cuenta de transferencia.")
                End If

                If oMonedaT_stro.Value.Trim = String.Empty Then
                    Throw New Exception("Moneda no definida en cuenta de transferencia.")
                End If

                If oTipoCuentaT_stro.Value.Trim = String.Empty Then
                    Throw New Exception("Tipo de cuenta no definida en cuenta de transferencia.")
                End If

                If oCuentaBancariaT_stro.Value.Trim = String.Empty Then
                    Throw New Exception("Cuanta bancaria no definido en cuenta de transferencia.")
                End If

            End If

            ValidarDatos = True

        Catch ex As Exception
            ValidarDatos = False
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("ValidarDatos : {0}", ex.Message), TipoMsg.Falla)
        End Try

    End Function
    Private Function BuscarControlPorID(ByVal rootControl As Control, ByVal controlID As String) As Control

        Try

            If rootControl.ID = controlID Then
                Return rootControl
            End If

            For Each controlToSearch As Control In rootControl.Controls

                Dim controlToReturn As Control = BuscarControlPorID(controlToSearch, controlID)

                If controlToReturn IsNot Nothing Then
                    Return controlToReturn
                End If

            Next

            Return Nothing

        Catch ex As Exception
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("BuscarControlPorID error: {0}", ex.Message), TipoMsg.Falla)
            Return Nothing
        End Try

    End Function
    Public Function BuscarControlPorClase(ByVal rootControl As Control, ByVal controlCss As String) As Control

        Dim control As Control = Nothing

        Try

            For Each controlToSearch As Control In rootControl.Controls

                Select Case controlToSearch.GetType().Name

                    Case "DropDownList"

                        Dim ddl As DropDownList = controlToSearch

                        If ddl.CssClass = controlCss Then
                            control = ddl
                            Exit For
                        End If

                    Case Else

                        Dim controlToReturn As Control = BuscarControlPorClase(controlToSearch, controlCss)

                        If controlToReturn IsNot Nothing Then
                            Return controlToReturn
                        End If

                End Select

            Next

        Catch ex As Exception
            control = Nothing
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("BuscarControlPorClase error: {0}", ex.Message), TipoMsg.Falla)
        End Try

        Return control

    End Function
    Public Function ObtenerTipoCambio() As Double

        Dim oDatos As DataSet

        Dim oParametros As New Dictionary(Of String, Object)

        Try


            oParametros = New Dictionary(Of String, Object)

            'oParametros.Add("TipoMoneda", Me.cmbMonedaPago.SelectedValue)
            oParametros.Add("TipoMoneda", 1)
            oParametros.Add("Fecha", IIf(String.IsNullOrWhiteSpace(Me.txtFechaContable.Text.Trim()), String.Empty, Me.txtFechaContable.Text.Trim))

            oDatos = New DataSet

            oDatos = Funciones.ObtenerDatos("usp_ObtenerUltimoCambio_stro", oParametros)

            If Not oDatos Is Nothing AndAlso oDatos.Tables(0).Rows.Count > 0 Then
                ObtenerTipoCambio = oDatos.Tables(0).Rows(0).Item(2)
            Else
                ObtenerTipoCambio = 1
            End If


        Catch ex As Exception
            ObtenerTipoCambio = 1
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("ObtenerTipoCambio error: {0}", ex.Message), TipoMsg.Falla)
        End Try

    End Function
    Public Function SiniestroAbierto(ByVal lSiniestro As Long, ByVal iSubsiniestro As Integer) As Boolean

        Dim oDatos As DataSet

        Dim oParametros As New Dictionary(Of String, Object)

        Try

            SiniestroAbierto = False

            oParametros = New Dictionary(Of String, Object)

            oParametros.Add("NumeroSiniestro", lSiniestro)
            oParametros.Add("Subsiniestro", iSubsiniestro)
            oParametros.Add("TipoEstimacion", 1)

            oDatos = New DataSet

            oDatos = Funciones.ObtenerDatos("ValidarSiniestroCerradoCancelado", oParametros)

            If Not oDatos Is Nothing AndAlso oDatos.Tables(0).Rows.Count > 0 Then
                SiniestroAbierto = IIf(oDatos.Tables(0).Rows(0).Item("Valido") = "Y", True, False)
            End If

        Catch ex As Exception
            SiniestroAbierto = False
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("ObtenerTipoCambio error: {0}", ex.Message), TipoMsg.Falla)
        End Try

    End Function
    Public Function ValidarFechaComprobante(ByVal sFecha As String, ByVal iSiniestro As Integer) As Boolean

        Dim oDatos As DataSet

        Dim oParametros As New Dictionary(Of String, Object)

        Try

            ValidarFechaComprobante = False

            oDatos = New DataSet

            oParametros = New Dictionary(Of String, Object)

            oParametros.Add("Fecha", Convert.ToDateTime(sFecha).ToString("yyyyMMdd"))
            oParametros.Add("Siniestro", iSiniestro)
            'Fecha de comprobante, menor a fecha ocurrencia stro(s)
            oDatos = Funciones.ObtenerDatos("usp_ValidarFechaComprobante_stro", oParametros)

            If Not oDatos Is Nothing AndAlso oDatos.Tables(0).Rows.Count > 0 Then
                ValidarFechaComprobante = IIf(oDatos.Tables(0).Rows(0).Item("Valido") = "Y", True, False)
            End If

        Catch ex As Exception
            ValidarFechaComprobante = False
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("ValidarFechaComprobante error: {0}", ex.Message), TipoMsg.Falla)
        End Try

    End Function
#End Region

#Region "Métodos"
    Public Sub InicializarValores()

        Try

            IniciaGrid()

            CargarBeneficiarios()

            CargarCatalogosCuentasBancarias()

            Me.cmbTipoUsuario.Enabled = True
            Me.cmbTipoUsuario.SelectedValue = eTipoUsuario.Asegurado

            Me.txtSiniestro.Text = String.Empty

            'If Me.cmbSubsiniestro.Items.Count > 0 Then
            '    Me.cmbSubsiniestro.Items.Clear()
            'End If

            Me.txtCodigoCuenta.Text = String.Empty
            Me.txtDescCuenta.Text = String.Empty
            Me.txtCodigoBeneficiario_stro.Text = String.Empty
            Me.txtBeneficiario_stro.Text = String.Empty
            Me.txtRFC.Text = String.Empty
            Me.cmbMonedaPago.SelectedValue = 0
            Me.txtTipoCambio.Text = "1.00"

            Me.grd.DataSource = Nothing
            Me.grd.DataBind()

            Me.txtTotalAutorizacion.Text = String.Empty
            'Me.txtTotalAutorizacionNacional.Text = String.Empty
            Me.txtTotalImpuestos.Text = String.Empty
            Me.txtTotalRetenciones.Text = String.Empty
            Me.txtTotal.Text = String.Empty

            Me.iptxtTotalAutorizacion.Text = String.Empty 'importe de pago
            Me.iptxtTotalImpuestos.Text = String.Empty
            Me.iptxtTotal.Text = String.Empty 'importe de pago

            Me.txtTotalAutorizacionFac.Text = String.Empty 'txt de facturas
            Me.txtTotalImpuestosFac.Text = String.Empty
            Me.txtTotalRetencionesFac.Text = String.Empty
            Me.txtTotalFac.Text = String.Empty
            Me.txtTotalAutorizacionNacionalFac.Text = String.Empty
            Me.txtDescuentos.Text = String.Empty 'txt de facturas

            Me.txtBeneficiario.Text = String.Empty
            Me.txtBeneficiario_stro.Enabled = True   ' lo cambie para que se habilite la busqueda

            If Me.cmbOrigendePago.Items.Count > 0 Then
                Me.cmbOrigendePago.Items.Clear()
            End If

            Me.cmbTipoPagoOP.SelectedValue = 0
            Me.txtDescripcionOP.Text = String.Empty
            Me.txtSiniestro.Enabled = True

            Me.txtFechaRegistro.Text = DateTime.Now.ToString("dd/MM/yyyy")
            Me.txtFechaEstimadaPago.Text = Me.txtFechaRegistro.Text

            Me.txtNumeroComprobante.Text = String.Empty
            Me.txtFechaComprobante.Text = String.Empty

            Me.txtFechaEstimadaPago.Style("text-align") = "center"
            Me.txtTipoCambio.Style("text-align") = "center"
            Me.txtTipoCambio.Style("width") = "80px"

            Me.btnVerCuentas.Visible = False

        Catch ex As Exception
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("InicializarValores error: {0}", ex.Message), TipoMsg.Falla)
        End Try

    End Sub
    Public Sub ObtenerDatosTransferenciaProveedor()

        Dim oParametros As New Dictionary(Of String, Object)

        Dim oDatos As DataSet

        Try

            If grd.Rows.Count > 0 AndAlso cmbTipoPagoOP.SelectedValue = -1 Then

                oDatos = New DataSet

                oParametros = New Dictionary(Of String, Object)

                oParametros.Add("Codigo", CInt(oGrdOrden.Rows(0).Item("IdPersona")))

                oDatos = Funciones.ObtenerDatos("usp_CargarDatosBancariosBeneficiario_stro", oParametros)

                If Not oDatos Is Nothing AndAlso oDatos.Tables(0).Rows.Count > 0 Then

                    With oDatos.Tables(0).Rows(0)

                        oBancoT_stro.Value = .Item("CodigoBanco")
                        oMonedaT_stro.Value = .Item("CodigoMoneda")
                        oTipoCuentaT_stro.Value = .Item("TipoCuenta")
                        oCuentaBancariaT_stro.Value = .Item("NumeroCuenta")
                        oBeneficiarioT_stro.Value = .Item("Beneficiario")

                    End With

                Else

                    If Me.cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then
                        Mensaje.MuestraMensaje("Cuentas bancarias", "No existen cuentas asociadas", TipoMsg.Falla)
                        Me.cmbTipoPagoOP.SelectedValue = 0
                        Me.btnVerCuentas.Visible = False
                    End If

                End If

                oSucursalT_stro.Value = "CIUDAD DE MEXICO"
                oBeneficiarioT_stro.Value = IIf(oBeneficiarioT_stro.Value = String.Empty, Me.txtBeneficiario.Text.Trim, oBeneficiarioT_stro.Value)

            End If

        Catch ex As Exception
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("ObtenerDatosTransferenciaProveedor error: {0}", ex.Message), TipoMsg.Falla)
        End Try

    End Sub
    Public Sub CalcularTotales()

        Dim dTotalAutorizacion As Double = 0
        Dim dTotalImpuestos As Double = 0
        Dim dTotalRetenciones As Double = 0
        Dim dPago As Double = 0
        Dim dTipoCambio As Double
        Dim dImporteImpuesto As Double = 0
        Dim dImporteRetencion As Double = 0
        Dim dDescuentos As Double = 0

        Dim dTotalAutorizacionNacional As Double = 0   'Utilizado solo para impuestos
        Dim dTotalImpuestosNacional As Double = 0

        Dim dcod_clase_pago As Int16 = 0
        Dim dcod_cpto As Int16 = 0
        Try

            If txtDescCuenta.Text = "NACIONAL" Then
                cmbMonedaPago.SelectedValue = 0
                dTipoCambio = 1
            Else
                dTipoCambio = ObtenerTipoCambio()
            End If

            Me.txtTipoCambio.Text = dTipoCambio

            For Each oFila In oGrdOrden.Rows

                'dPago = IIf(IsDBNull(oFila("Pago")), 0, oFila("Pago"))
                dPago = IIf(IsDBNull(oFila("Subtotal")), 0, oFila("Subtotal"))
                dcod_clase_pago = IIf(IsDBNull(oFila("ClasePago")), 0, oFila("ClasePago"))
                dcod_cpto = IIf(IsDBNull(oFila("ConceptoPago")), 0, oFila("ConceptoPago"))
                dDescuentos = IIf(IsDBNull(oFila("Descuentos")), 0, oFila("Descuentos"))

                If cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then

                    dPago = dPago - dDescuentos
                    txtTotalAutorizacionFac.Text = String.Format("{0:0,0.00}", Math.Round(Decimal.Parse(txtTotalAutorizacionFac.Text) - dDescuentos), 2)
                    If dPago > 0 Then

                        'Si es un proveedor cuya factura haya sido registrada en pesos, se tomara la moneda de pago en pesos
                        If oFila("MonedaFactura") = 0 Then

                            If Not dTipoCambio = 1 Then
                                Mensaje.MuestraMensaje("Calculo de totales", "Factura capturada en pesos, se utilizará tipo de cambio nacional.", TipoMsg.Advertencia)
                                'dTipoCambio = 1
                                'Me.txtTipoCambio.Text = dTipoCambio
                                cmbMonedaPago.SelectedValue = 0
                            End If

                        End If
                        'ObtenerImpuestos(CInt(Me.txtCodigoBeneficiario_stro.Text), CInt(oFila("ClasePago")), CInt(oFila("ConceptoPago")), CInt(oFila("IdSiniestro")), dPago, dImporteImpuesto, dImporteRetencion)
                        ObtenerImpuestosFondos(CInt(Me.txtcod_pres.Text), CInt(oFila("ClasePago")), CInt(oFila("ConceptoPago")), CInt(oFila("IdSiniestro")), dPago, dImporteImpuesto, dImporteRetencion)

                        If dImporteImpuesto = -1 AndAlso dImporteRetencion = -1 Then
                            'Mensaje.MuestraMensaje("Calculo de totales", "No se encontro información para el cálculo de impuestos", TipoMsg.Falla)
                            'dPago = 0
                            'dImporteImpuesto = 0
                            'dImporteRetencion = 0
                            Mensaje.MuestraMensaje("Calculo de totales", "No se encontro información para el cálculo de impuestos", TipoMsg.Falla)
                            txtTotalAutorizacionNacionalFac.Text = String.Format("{0:0,0.00}", Math.Round(0, 2))
                            txtTotalAutorizacionFac.Text = String.Format("{0:0,0.00}", Math.Round(0, 2))
                            txtTotalImpuestosFac.Text = String.Format("{0:0,0.00}", Math.Round(0, 2))
                            txtTotalRetencionesFac.Text = String.Format("{0:0,0.00}", Math.Round(0, 2))
                            txtTotalFac.Text = String.Format("{0:0,0.00}", Math.Round(0, 2))
                            txtTotalNacionalFac.Text = String.Format("{0:0,0.00}", Math.Round(0, 2))
                            If dcod_clase_pago = 26 AndAlso dImporteImpuesto = -1 AndAlso dImporteRetencion = -1 Then
                                txtTotalAutorizacion.Text = dPago
                                txtTotalImpuestos.Text = 0
                                txtTotalRetenciones.Text = 0
                                txtTotal.Text = dPago
                                txtTotalNacional.Text = dPago

                                txtTotalAutorizacionNacionalFac.Text = String.Format("{0:0,0.00}", Math.Round(dPago, 2))
                                txtTotalAutorizacionFac.Text = String.Format("{0:0,0.00}", Math.Round(dPago, 2))
                                txtTotalImpuestosFac.Text = String.Format("{0:0,0.00}", Math.Round(0, 2))
                                txtTotalRetencionesFac.Text = String.Format("{0:0,0.00}", Math.Round(0, 2))
                                txtTotalFac.Text = String.Format("{0:0,0.00}", Math.Round(dPago, 2))
                                txtTotalNacionalFac.Text = String.Format("{0:0,0.00}", Math.Round(dPago, 2))

                            End If
                            dImporteImpuesto = 0
                            dImporteRetencion = 0
                        ElseIf (dImporteImpuesto = 0 AndAlso dImporteRetencion = 0) OrElse
                            (dImporteImpuesto = -1 OrElse dImporteRetencion = -1) Then
                            Mensaje.MuestraMensaje("Calculo de totales", "Cálculo incompleto de impuestos", TipoMsg.Falla)
                            Return
                        End If

                    Else
                        dPago = 0
                        dImporteImpuesto = 0
                        dImporteRetencion = 0
                    End If

                End If

                Select Case cmbMonedaPago.SelectedValue

                    Case 0  'NACIONAL

                        Select Case oFila("TipoMoneda")
                            Case 0
                                dTotalAutorizacion += dPago
                                dTotalImpuestos += IIf(cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor, dImporteImpuesto, 0)
                                dTotalRetenciones += IIf(cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor, dImporteRetencion, 0)

                                dTotalAutorizacionNacional += dPago
                                dTotalImpuestosNacional = 0

                            Case 1
                                dTotalAutorizacion += dPago
                                dTotalImpuestos += IIf(cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor, (dImporteImpuesto), 0)
                                dTotalRetenciones += IIf(cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor, (dImporteRetencion), 0)

                                dTotalAutorizacionNacional += dPago
                                dTotalImpuestosNacional += (dImporteImpuesto - dImporteRetencion)

                        End Select

                    Case 1  'DOLARES

                        Select Case oFila("TipoMoneda")

                            'Solo se pueden pagar con dolares una póliza que este en dólares
                            Case 1
                                dTotalAutorizacion += dPago
                                dTotalImpuestos += IIf(cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor, dImporteImpuesto, 0)
                                dTotalRetenciones += IIf(cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor, dImporteRetencion, 0)

                                'Como la póliza esta en dólares, se obtendra el valor en pesos para la autorización nacional.
                                dTotalAutorizacionNacional += dPago

                            Case Else
                                dTotalAutorizacion = 0
                                dTotalImpuestos = 0
                                dTotalRetenciones = 0
                                dTotalAutorizacionNacional = 0
                                'dTotalNacional += IIf(cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor, (dPago + dImporteImpuesto - dImporteRetencion), 0)

                        End Select

                End Select

            Next

            Select Case cmbTipoUsuario.SelectedValue
                Case eTipoUsuario.Asegurado, eTipoUsuario.Tercero

                    'Cambiara segun si la moneda de pago son pesos o dolares
                    Me.txtTotalAutorizacion.Text = String.Format("{0:0,0.00}", Math.Round(dTotalAutorizacion, 2))
                    Me.iptxtTotalAutorizacion.Text = String.Format("{0:0,0.00}", Math.Round(dTotalAutorizacion, 2))

                    'Si la moneda de la póliza es nacional se obtendran total de pago en pesos, si no los tomará del total de autorización
                    'que seran dolares
                    'Me.iptxtTotalAutorizacion.Text = String.Format("{0:0,0.00}", Math.Round(IIf(Me.txtMonedaPoliza.Text = "NACIONAL", dTotalAutorizacion, dTotalAutorizacionNacional), 2))

                    'Los impuestos y retenciones deben ser cero para asegurados y terceros
                    Me.txtTotalImpuestos.Text = String.Format("{0:0,0.00}", Math.Round(dTotalImpuestos, 2))
                    Me.iptxtTotalImpuestos.Text = String.Format("{0:0,0.00}", Math.Round(dTotalImpuestos, 2))
                    Me.txtTotalRetenciones.Text = String.Format("{0:0,0.00}", Math.Round(dTotalRetenciones, 2))
                    Me.iptxtTotalRetenciones.Text = String.Format("{0:0,0.00}", Math.Round(dTotalRetenciones, 2))

                    'El valor del total sera el total en la moneda la cual este registrada la póliza
                    Me.txtTotal.Text = String.Format("{0:0,0.00}", Math.Round(dTotalAutorizacion + dTotalImpuestos - dTotalRetenciones, 2))
                    Me.iptxtTotal.Text = String.Format("{0:0,0.00}", Math.Round(dTotalAutorizacion + dTotalImpuestos - dTotalRetenciones, 2))

                    Me.txtTotalNacional.Text = String.Format("{0:0,0.00}", dTotalAutorizacionNacional)
                    Me.iptxtTotalNacional.Text = String.Format("{0:0,0.00}", dTotalAutorizacionNacional)

                Case eTipoUsuario.Proveedor
                    Me.txtTotalAutorizacion.Text = String.Format("{0:0,0.00}", Math.Round(dTotalAutorizacion, 2))
                    Me.txtTotalImpuestos.Text = String.Format("{0:0,0.00}", Math.Round(dTotalImpuestos, 2))
                    Me.txtTotalRetenciones.Text = String.Format("{0:0,0.00}", Math.Round(dTotalRetenciones, 2))
                    Me.txtTotal.Text = String.Format("{0:0,0.00}", Math.Round(dTotalAutorizacion + dTotalImpuestos - dTotalRetenciones, 2))
                    Me.txtTotalNacional.Text = String.Format("{0:0,0.00}", Math.Round(dTotalAutorizacionNacional + dTotalImpuestosNacional, 2))

                    Me.iptxtTotalAutorizacion.Text = String.Format("{0:0,0.00}", Math.Round(dTotalAutorizacion, 2))
                    Me.iptxtTotalImpuestos.Text = String.Format("{0:0,0.00}", Math.Round(dTotalImpuestos, 2))
                    Me.iptxtTotalRetenciones.Text = String.Format("{0:0,0.00}", Math.Round(dTotalRetenciones, 2))
                    Me.iptxtTotal.Text = String.Format("{0:0,0.00}", Math.Round((dTotalAutorizacion) + (dTotalImpuestos) - (dTotalRetenciones), 2))
                    Me.iptxtTotalNacional.Text = String.Format("{0:0,0.00}", Math.Round((dTotalAutorizacionNacional) + (dTotalImpuestosNacional), 2))
                    'Me.iptxtTotalAutorizacionNacional.Text = String.Format("{0:0,0.00}", Math.Round(dTotalAutorizacion * dTipoCambio, 2))

                    Me.txtDescuentos.Text = dDescuentos
            End Select

        Catch ex As Exception
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("CalcularTotales error: {0}", ex.Message), TipoMsg.Falla)
        End Try

    End Sub
    Public Sub ObtenerImpuestosFondos(ByVal iCodigoProveedor As Integer, ByVal iClasePago As Integer, ByVal iConceptoPago As Integer, ByVal iIdSiniestro As Integer, ByVal dMonto As Double, ByRef dImpuesto As Double, ByRef dRetencion As Double)

        Dim oDatos As DataSet

        Dim oParametros As New Dictionary(Of String, Object)

        Try

            oParametros = New Dictionary(Of String, Object)

            oParametros.Add("CodPres", iCodigoProveedor)
            oParametros.Add("ClasePago", iClasePago)
            oParametros.Add("ConceptoPago", iConceptoPago)
            oParametros.Add("IdStro", 1) 'iIdSiniestro)
            oParametros.Add("Monto", dMonto)
            oParametros.Add("SoloLectura", "-1")

            oDatos = New DataSet

            oDatos = Funciones.ObtenerDatos("usp_ObtenerImpuestos_stro", oParametros)

            If Not oDatos Is Nothing AndAlso oDatos.Tables(0).Rows.Count > 0 Then
                dImpuesto = CDbl(oDatos.Tables(0).Rows(0).Item("ImporteImpuesto"))
                dRetencion = CDbl(oDatos.Tables(0).Rows(0).Item("PjeRetencion"))
                'txtTotalImpuestos.Text = CDbl(oDatos.Tables(0).Rows(0).Item("ImporteImpuesto"))
                'txtTotalRetenciones.Text = CDbl(oDatos.Tables(0).Rows(0).Item("PjeRetencion"))

            Else
                dImpuesto = -1
                dRetencion = -1
            End If

        Catch ex As Exception
            dImpuesto = -1
            dRetencion = -1
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("ObtenerImpuestos error: {0}", ex.Message), TipoMsg.Falla)
        End Try

    End Sub
    Public Sub ObtenerImpuestos(ByVal iCodigoProveedor As Integer, ByVal iClasePago As Integer, ByVal iConceptoPago As Integer, ByVal iIdSiniestro As Integer, ByVal dMonto As Double, ByRef dImpuesto As Double, ByRef dRetencion As Double)

        Dim oDatos As DataSet

        Dim oParametros As New Dictionary(Of String, Object)

        Try

            oParametros = New Dictionary(Of String, Object)

            oParametros.Add("CodPres", iCodigoProveedor)
            oParametros.Add("ClasePago", iClasePago)
            oParametros.Add("ConceptoPago", iConceptoPago)
            oParametros.Add("IdStro", iIdSiniestro)
            oParametros.Add("Monto", dMonto)
            oParametros.Add("SoloLectura", "Y")

            oDatos = New DataSet

            oDatos = Funciones.ObtenerDatos("usp_ObtenerImpuestos_stro", oParametros)

            If Not oDatos Is Nothing AndAlso oDatos.Tables(0).Rows.Count > 0 Then
                dImpuesto = CDbl(oDatos.Tables(0).Rows(0).Item("TotalImpuestos"))
                dRetencion = CDbl(oDatos.Tables(0).Rows(0).Item("TotalRetenciones"))
            Else
                dImpuesto = -1
                dRetencion = -1
            End If

        Catch ex As Exception
            dImpuesto = -1
            dRetencion = -1
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("ObtenerImpuestos error: {0}", ex.Message), TipoMsg.Falla)
        End Try

    End Sub
    Public Sub ObtenerDetalleImpuestos(ByRef oDatos As DataSet, ByVal iCodigoProveedor As Integer, ByVal iClasePago As Integer, ByVal iConceptoPago As Integer, ByVal iIdSiniestro As Integer, ByVal dMonto As Double)

        Dim oParametros As New Dictionary(Of String, Object)

        Try
            oParametros = New Dictionary(Of String, Object)
            oParametros.Add("CodPres", iCodigoProveedor)
            oParametros.Add("ClasePago", iClasePago)
            oParametros.Add("ConceptoPago", iConceptoPago)
            oParametros.Add("IdStro", iIdSiniestro)
            oParametros.Add("Monto", dMonto)
            oParametros.Add("SoloLectura", "N")

            oDatos = Funciones.ObtenerDatos("usp_ObtenerImpuestos_stro", oParametros)

        Catch ex As Exception
            oDatos = Nothing
            Throw ex
        End Try
    End Sub
    Public Function ValidarImpuestosOPFac() As Boolean

        Dim iTotalAutorizacion As Decimal
        Dim iTotalImpuestosn As Decimal
        Dim iTotalRetenciones As Decimal
        Dim iSubTotal As Decimal

        Try
            If cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then
                'Validar los impuestos y totales de la factura con el calculo del sii
                If txtMonedaPoliza.Text = "NACIONAL" Then
                    iTotalAutorizacion = (Decimal.Parse(iptxtTotalAutorizacion.Text) + txtDescuentos.Text) - Decimal.Parse(txtTotalAutorizacionFac.Text)
                    iTotalImpuestosn = Decimal.Parse(iptxtTotalImpuestos.Text) - Decimal.Parse(txtTotalImpuestosFac.Text)
                    iTotalRetenciones = Decimal.Parse(iptxtTotalRetenciones.Text) - Decimal.Parse(txtTotalRetencionesFac.Text)
                    iSubTotal = Decimal.Parse(iptxtTotal.Text) - Decimal.Parse(txtTotalFac.Text)
                Else
                    iTotalAutorizacion = (Decimal.Parse(txtTotalAutorizacionFac.Text)) + (Decimal.Parse(txtDescuentos.Text)) - Decimal.Parse(txtTotalAutorizacion.Text)
                    iTotalImpuestosn = (Decimal.Parse(txtTotalImpuestosFac.Text)) - Decimal.Parse(txtTotalImpuestos.Text)
                    iTotalRetenciones = (Decimal.Parse(txtTotalRetencionesFac.Text)) - Decimal.Parse(txtTotalRetenciones.Text)
                    iSubTotal = (Decimal.Parse(txtTotalFac.Text)) - Decimal.Parse(txtTotal.Text)
                End If

                '--Autorizacion
                If Math.Abs(iTotalAutorizacion) > 0.5 Then
                    ValidarImpuestosOPFac = False
                    Mensaje.MuestraMensaje("OrdenPagoSiniestros", "Diferencia de Autorizacion: " + iTotalAutorizacion.ToString(), TipoMsg.Falla)
                Else
                    '--impuestos
                    If Math.Abs(iTotalImpuestosn) > 0.5 Then
                        ValidarImpuestosOPFac = False
                        Mensaje.MuestraMensaje("OrdenPagoSiniestros", "Diferencia de iTotalImpuestosn: " + iTotalImpuestosn.ToString(), TipoMsg.Falla)
                    Else
                        '--retenciones
                        If Math.Abs(iTotalRetenciones) > 0.5 Then
                            ValidarImpuestosOPFac = False
                            Mensaje.MuestraMensaje("OrdenPagoSiniestros", "Diferencia de iTotalRetenciones: " + iTotalRetenciones.ToString(), TipoMsg.Falla)
                        Else
                            '--subtotal
                            If Math.Abs(iSubTotal) > 0.5 Then
                                ValidarImpuestosOPFac = False
                                Mensaje.MuestraMensaje("OrdenPagoSiniestros", "Diferencia de iSubTotal: " + iSubTotal.ToString(), TipoMsg.Falla)
                            Else
                                'Debe estar en true, esto significa que no ubo diferencias en los impuestos
                                ValidarImpuestosOPFac = True
                            End If
                        End If
                    End If
                End If
            Else
                'para el caso de asegurado y terceros que no tienen descuentos
                ValidarImpuestosOPFac = True
            End If
        Catch ex As Exception
            ValidarImpuestosOPFac = False
        End Try
    End Function
    Public Sub GenerarOrdenPago() Handles btnGrabarOP.Click
        Dim oDatos As DataSet
        oDatos = New DataSet
        Dim oParametros As New Dictionary(Of String, Object)
        Try
            If ValidarImpuestosOPFac() = True Then
                'Validamos si es un multipago 
                If oGrdOrden.Rows.Count = 0 Then 'cambiar a uno si queremos validar que este lleno el grid
                    'No es multipago
                    If cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then
                        'PROVEEDOR
                        oParametros.Add("Pagar_A", "2")
                        oParametros.Add("sn_transferencia", cmbTipoPagoOP.SelectedValue)
                        oParametros.Add("cod_modulo", "4")
                        oParametros.Add("nro_correlativo", "0")
                        oParametros.Add("txt_clave", Me.txtCodigoCuenta.Text.Trim)
                        oParametros.Add("cod_moneda", CInt(cmbMonedaPago.SelectedValue))
                        oParametros.Add("imp_mo", Me.iptxtTotal.Text)
                        oParametros.Add("imp_eq", Me.iptxtTotal.Text)
                        oParametros.Add("imp_cambio", CDbl(txtTipoCambio.Text))
                        oParametros.Add("txt_desc", Me.txtDescripcionOP.Text + " " + txtcpto2.Text)
                        oParametros.Add("cod_suc", CInt(cmbSucursal.SelectedValue))
                        oParametros.Add("cod_usuario", IIf(Master.cod_usuario = String.Empty, "SISE", Master.cod_usuario))
                        oParametros.Add("fec_estim_pago", Convert.ToDateTime(txtFechaEstimadaPago.Text.Trim).ToString("yyyyMMdd"))
                        oParametros.Add("D_C", "D")
                        oParametros.Add("cod_concepto_cble", CInt(cmbConcepto.SelectedValue))
                        oParametros.Add("cod_sector", 5) 'estaba el 8
                        oParametros.Add("id_persona", Me.txtCodigoBeneficiario_stro.Text.Trim)
                        oParametros.Add("nombre_persona", Me.txtBeneficiario_stro.Text.Trim)
                        'Trasferencia
                        If cmbTipoPagoOP.SelectedValue = -1 Then
                            oParametros.Add("id_cuenta_bancaria", oBancoT_stro.Value)
                            oParametros.Add("nro_cuenta_transferencia", oCuentaBancariaT_stro.Value)
                            oParametros.Add("cod_banco_transferencia", IIf(oBancoT_stro.Value = String.Empty, String.Empty, CInt(oBancoT_stro.Value)))
                            'Datos Proveedor
                            oParametros.Add("cod_tipo_doc", CInt(cmbTipoDocumento.SelectedValue))
                            oParametros.Add("nro_doc", " ")
                            oParametros.Add("cod_origen_pago", CInt(cmbOrigendePago.SelectedValue))
                            oParametros.Add("nro_comprobante_proveedor", txtNumeroComprobante.Text)
                        Else 'CHEKE
                            oParametros.Add("id_cuenta_bancaria", 0)
                            oParametros.Add("nro_cuenta_transferencia", " ")
                            oParametros.Add("cod_banco_transferencia", 0)
                            'Datos Proveedor
                            oParametros.Add("cod_tipo_doc", 0)
                            oParametros.Add("nro_doc", " ") ''
                            oParametros.Add("cod_origen_pago", CInt(cmbOrigendePago.SelectedValue))
                            oParametros.Add("nro_comprobante_proveedor", " ")
                        End If
                        oParametros.Add("Analista_Fondos", cmbAnalistaSolicitante.SelectedValue)
                        oDatos = Funciones.ObtenerDatos("MIS_sp_cir_op_stro_grabar_op_numero_siniestro", oParametros)
                        If (oDatos.Tables(0).Rows(0).Item("Mensaje") = "1") Then
                            'Impresión reporte
                            Dim ws As New ws_Generales.GeneralesClient
                            Dim server As String = ws.ObtieneParametro(3)
                            server = Replace(Replace(server, "@Reporte", "OrdenPago"), "@Formato", "PDF") & "&nro_op=@nro_op"
                            server = Replace(server, "ReportesGMX_DESA", "ReportesOPSiniestros_DESA")
                            server = Replace(server, "OrdenPago", "OrdenPago_stro")
                            'Funciones.EjecutaFuncion("fn_ImprimirOrden('" & server & "','" & "234777" & "');")
                            Funciones.EjecutaFuncion(String.Format("fn_ImprimirOrden('{0}','{1}');", server, CStr(oDatos.Tables(oDatos.Tables.Count - 1).Rows(0).Item("nro_op"))))

                            Mensaje.MuestraMensaje("SINIESTROS", String.Format("Solicitud de pago: {0} Orden de pago: {1}",
                                                                                    oDatos.Tables(oDatos.Tables.Count - 1).Rows(0).Item("SP"),
                                                                                    oDatos.Tables(oDatos.Tables.Count - 1).Rows(0).Item("nro_op")), TipoMsg.Confirma)
                        Else
                            Mensaje.MuestraMensaje("Error: ", oDatos.Tables(0).Rows(0).Item("Mensaje").ToString(), TipoMsg.Falla)
                        End If
                    Else
                        If ValidarDatos() Then
                            'ASEGURADO, TERCERO
                            oParametros.Add("Pagar_A", 1)
                            oParametros.Add("sn_transferencia", CInt(cmbTipoPagoOP.SelectedValue))
                            oParametros.Add("cod_modulo", 4)
                            oParametros.Add("nro_correlativo", 0)
                            oParametros.Add("txt_clave", Me.txtCodigoCuenta.Text.Trim.ToString())
                            oParametros.Add("cod_moneda", CInt(cmbMonedaPago.SelectedValue))
                            oParametros.Add("imp_mo", CDbl(Me.iptxtTotal.Text))
                            oParametros.Add("imp_eq", CDbl(Me.iptxtTotal.Text))
                            oParametros.Add("imp_cambio", CDbl(txtTipoCambio.Text))
                            oParametros.Add("txt_desc", Me.txtDescripcionOP.Text.ToString())
                            oParametros.Add("cod_suc", CInt(cmbSucursal.SelectedValue))
                            oParametros.Add("cod_usuario", IIf(Master.cod_usuario = String.Empty, "SISE", Master.cod_usuario))
                            oParametros.Add("fec_estim_pago", Convert.ToDateTime(txtFechaEstimadaPago.Text.Trim).ToString("yyyyMMdd"))
                            oParametros.Add("D_C", "D")
                            oParametros.Add("cod_concepto_cble", CInt(Me.cmbConcepto.Text.Trim))
                            oParametros.Add("cod_sector", 5) 'estaba el 8
                            oParametros.Add("id_persona", CInt(Me.txtCodigoBeneficiario_stro.Text.Trim))
                            oParametros.Add("nombre_persona", Me.txtBeneficiario_stro.Text.Trim.ToString())
                            'Trasferencia
                            If cmbTipoPagoOP.SelectedValue = -1 Then
                                oParametros.Add("id_cuenta_bancaria", oBancoT_stro.Value)
                                oParametros.Add("nro_cuenta_transferencia", oCuentaBancariaT_stro.Value)
                                oParametros.Add("cod_banco_transferencia", IIf(oBancoT_stro.Value = String.Empty, String.Empty, CInt(oBancoT_stro.Value)))
                                'Datos Proveedor
                                oParametros.Add("cod_tipo_doc", CInt(cmbTipoDocumento.SelectedValue))
                                oParametros.Add("nro_doc", "NULL")
                                oParametros.Add("cod_origen_pago", CInt(cmbOrigendePago.SelectedValue))
                                oParametros.Add("nro_comprobante_proveedor", CInt(txtNumeroComprobante.Text))
                            Else 'CHEKE
                                oParametros.Add("id_cuenta_bancaria", 0)
                                oParametros.Add("nro_cuenta_transferencia", " ")
                                oParametros.Add("cod_banco_transferencia", 0)
                                'Datos Proveedor
                                oParametros.Add("cod_tipo_doc", 0)
                                oParametros.Add("nro_doc", " ") ''
                                oParametros.Add("cod_origen_pago", CInt(cmbOrigendePago.SelectedValue))
                                oParametros.Add("nro_comprobante_proveedor", " ")
                            End If
                            oParametros.Add("Analista_Fondos", cmbAnalistaSolicitante.SelectedValue)
                            oDatos = Funciones.ObtenerDatos("MIS_sp_cir_op_stro_grabar_op_numero_siniestro", oParametros)
                            If (oDatos.Tables(0).Rows(0).Item("Mensaje") = "1") Then
                                'Impresión reporte
                                Dim ws As New ws_Generales.GeneralesClient
                                Dim server As String = ws.ObtieneParametro(3)
                                server = Replace(Replace(server, "@Reporte", "OrdenPago"), "@Formato", "PDF") & "&nro_op=@nro_op"
                                server = Replace(server, "ReportesGMX_DESA", "ReportesOPSiniestros_DESA")
                                server = Replace(server, "OrdenPago", "OrdenPago_stro")
                                'Funciones.EjecutaFuncion("fn_ImprimirOrden('" & server & "','" & "234777" & "');")
                                Funciones.EjecutaFuncion(String.Format("fn_ImprimirOrden('{0}','{1}');", server, CStr(oDatos.Tables(oDatos.Tables.Count - 1).Rows(0).Item("nro_op"))))

                                Mensaje.MuestraMensaje("SINIESTROS", String.Format("Orden de pago: {1}",
                                                                                        oDatos.Tables(oDatos.Tables.Count - 1).Rows(0).Item("Mensaje"),
                                                                                        oDatos.Tables(oDatos.Tables.Count - 1).Rows(0).Item("nro_op")), TipoMsg.Confirma)
                            Else
                                Mensaje.MuestraMensaje("Error: ", oDatos.Tables(0).Rows(0).Item("Mensaje").ToString(), TipoMsg.Falla)
                            End If
                        End If
                    End If
                Else
                    'Multipago
                    'PROVEEDOR
                    oParametros.Add("Pagar_A", "2")
                    oParametros.Add("sn_transferencia", cmbTipoPagoOP.SelectedValue)
                    oParametros.Add("cod_modulo", "4")
                    oParametros.Add("nro_correlativo", "0")
                    oParametros.Add("txt_clave", Me.txtCodigoCuenta.Text.Trim)
                    oParametros.Add("cod_moneda", CInt(cmbMonedaPago.SelectedValue))
                    oParametros.Add("imp_mo", CDbl(Me.iptxtTotal.Text))     'impuestos + subtotal + retencion
                    oParametros.Add("imp_eq", CDbl(Me.iptxtTotal.Text))     'impuestos + subtotal + retencion
                    'oParametros.Add("imp_eq", Me.txtTotalImpuestos.Text)            'impuestos
                    'oParametros.Add("imp_eq", Me.txtTotalAutorizacionNacional.Text) 'subtotal
                    'oParametros.Add("imp_eq", Me.txtTotalRetenciones.Text)          'retencion
                    oParametros.Add("imp_cambio", CDbl(txtTipoCambio.Text))
                    oParametros.Add("txt_desc", Me.txtDescripcionOP.Text)
                    oParametros.Add("cod_suc", CInt(cmbSucursal.SelectedValue))
                    oParametros.Add("cod_usuario", IIf(Master.cod_usuario = String.Empty, "SISE", Master.cod_usuario))
                    oParametros.Add("fec_estim_pago", Convert.ToDateTime(txtFechaEstimadaPago.Text.Trim).ToString("yyyyMMdd"))
                    oParametros.Add("D_C", "D")
                    oParametros.Add("cod_concepto_cble", CInt(cmbConcepto.SelectedValue))
                    oParametros.Add("cod_sector", 5) 'estaba el 8
                    oParametros.Add("id_persona", Me.txtCodigoBeneficiario_stro.Text.Trim)
                    oParametros.Add("nombre_persona", Me.txtBeneficiario_stro.Text.Trim)
                    'Trasferencia
                    If cmbTipoPagoOP.SelectedValue = -1 Then
                        oParametros.Add("id_cuenta_bancaria", oBancoT_stro.Value)
                        oParametros.Add("nro_cuenta_transferencia", oCuentaBancariaT_stro.Value)
                        oParametros.Add("cod_banco_transferencia", IIf(oBancoT_stro.Value = String.Empty, String.Empty, CInt(oBancoT_stro.Value)))
                        'Datos Proveedor
                        oParametros.Add("cod_tipo_doc", CInt(cmbTipoDocumento.SelectedValue))
                        oParametros.Add("nro_doc", " ")
                        oParametros.Add("cod_origen_pago", CInt(cmbOrigendePago.SelectedValue))
                        oParametros.Add("nro_comprobante_proveedor", txtNumeroComprobante.Text)
                    Else 'CHEKE
                        oParametros.Add("id_cuenta_bancaria", 0)
                        oParametros.Add("nro_cuenta_transferencia", " ")
                        oParametros.Add("cod_banco_transferencia", 0)
                        'Datos Proveedor
                        oParametros.Add("cod_tipo_doc", 0)
                        oParametros.Add("nro_doc", " ") ''
                        oParametros.Add("cod_origen_pago", CInt(cmbOrigendePago.SelectedValue))
                        oParametros.Add("nro_comprobante_proveedor", " ")
                    End If
                    oParametros.Add("Analista_Fondos", cmbAnalistaSolicitante.SelectedValue)
                    oDatos = Funciones.ObtenerDatos("MIS_sp_cir_op_stro_grabar_op_numero_siniestro", oParametros)
                    If (oDatos.Tables(0).Rows(0).Item("Mensaje") = "1") Then
                        'Impresión reporte
                        Dim ws As New ws_Generales.GeneralesClient
                        Dim server As String = ws.ObtieneParametro(3)
                        server = Replace(Replace(server, "@Reporte", "OrdenPago"), "@Formato", "PDF") & "&nro_op=@nro_op"
                        server = Replace(server, "ReportesGMX_DESA", "ReportesOPSiniestros_DESA")
                        server = Replace(server, "OrdenPago", "OrdenPago_stro")
                        'Funciones.EjecutaFuncion("fn_ImprimirOrden('" & server & "','" & "234777" & "');")
                        Funciones.EjecutaFuncion(String.Format("fn_ImprimirOrden('{0}','{1}');", server, CStr(oDatos.Tables(oDatos.Tables.Count - 1).Rows(0).Item("nro_op"))))

                        Mensaje.MuestraMensaje("SINIESTROS", String.Format("Solicitud de pago: {0} Orden de pago: {1}",
                                                                                oDatos.Tables(oDatos.Tables.Count - 1).Rows(0).Item("SP"),
                                                                                oDatos.Tables(oDatos.Tables.Count - 1).Rows(0).Item("nro_op")), TipoMsg.Confirma)
                    Else
                        Mensaje.MuestraMensaje("Error: ", oDatos.Tables(0).Rows(0).Item("Mensaje").ToString(), TipoMsg.Falla)
                    End If
                End If
            End If
        Catch ex As Exception
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("GenerarOrdenPago error: {0}", ex.Message), TipoMsg.Falla)
        End Try
    End Sub
    Public Sub CargarCatalogosCuentasBancarias()

        Dim oDatos As DataSet

        Try

            oDatos = New DataSet

            oDatos = Funciones.ObtenerDatos("usp_ObtenerCatalogosCuentasBancarias_stro")

            If Not oDatos Is Nothing Then

                If oDatos.Tables(0).Rows.Count = 0 OrElse
                    oDatos.Tables(1).Rows.Count = 0 OrElse
                    oDatos.Tables(2).Rows.Count = 0 Then
                    Throw New Exception("Error al cargar catalogos de cuentas bancarias")
                Else
                    oDatos.Tables(0).TableName = "Bancos"
                    oDatos.Tables(1).TableName = "TiposCuenta"
                    oDatos.Tables(2).TableName = "Monedas"
                End If

                oCatalogoBancosT = oDatos.Tables("Bancos")
                oCatalogoTiposCuentaT = oDatos.Tables("TiposCuenta")
                oCatalogoMonedasT = oDatos.Tables("Monedas")

            End If

        Catch ex As Exception
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("CargarCatalogosCuentasBancarias error: {0}", ex.Message), TipoMsg.Falla)
        End Try

    End Sub
    Public Sub CargarConceptosPago(ByVal oRegistro As Control, ByVal iFila As Integer, ByVal sValor As String)

        Dim oParametros As New Dictionary(Of String, Object)

        Dim cmbConceptoPago As DropDownList

        Dim oDatos As DataSet

        Try

            oParametros = New Dictionary(Of String, Object)

            oDatos = New DataSet

            cmbConceptoPago = New DropDownList

            cmbConceptoPago = BuscarControlPorClase(oRegistro, "estandar-control concepto_pago")

            If Not cmbConceptoPago Is Nothing Then

                oParametros.Add("TipoUsuario", Me.cmbTipoUsuario.SelectedValue)
                oParametros.Add("ClasePago", CInt(sValor))
                oParametros.Add("CodigoPres", Me.txtCodigoBeneficiario_stro.Text)

                'oDatos = Funciones.ObtenerDatos("usp_ObtenerConceptosPago_stro", oParametros)
                oDatos = Funciones.ObtenerDatos("usp_ObtenerConceptosPago_stro_OPWEB", oParametros) 'FJCP_10290_CC

                If Not oDatos Is Nothing AndAlso oDatos.Tables(0).Rows.Count > 0 Then

                    If cmbConceptoPago.Items.Count > 0 Then
                        cmbConceptoPago.Items.Clear()
                    End If

                    cmbConceptoPago.DataSource = oDatos
                    cmbConceptoPago.DataTextField = "Descripcion"
                    cmbConceptoPago.DataValueField = "Concepto"
                    cmbConceptoPago.DataBind()

                    If Not cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then
                        'oGrdOrden.Rows(iFila)("ConceptoPago") = oDatos.Tables(0).Rows(0).Item("Concepto")
                        oGrdOrden.Rows(iFila)("ConceptoPago") = IIf(oGrdOrden.Rows(iFila)("ClasePago") = "26", "350", oDatos.Tables(0).Rows(0).Item("Concepto"))
                    Else

                        'If oDatos.Tables(0).Rows.Count = 1 Then
                        '    oGrdOrden.Rows(iFila)("ConceptoPago") = oDatos.Tables(0).Rows(0).Item("Concepto")
                        'End If

                        'If Not oGrdOrden.Rows(iFila)("ClasePago") = "26" Then
                        '    oGrdOrden.Rows(iFila)("ConceptoPago") = oDatos.Tables(0).Rows(0).Item("Concepto")
                        'Else

                        'End If
                        oGrdOrden.Rows(iFila)("ConceptoPago") = oDatos.Tables(0).Rows(0).Item("Concepto")
                    End If

                    If Not String.IsNullOrWhiteSpace(oGrdOrden.Rows(iFila)("ConceptoPago").ToString()) Then
                        cmbConceptoPago.Items.FindByValue(oGrdOrden.Rows(iFila)("ConceptoPago")).Selected = True
                    End If
                Else
                    oGrdOrden.Rows(iFila)("ConceptoPago") = ""
                    cmbConceptoPago.Items.Clear()
                End If

            End If

        Catch ex As Exception
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("CargarConceptosPago error: {0}", ex.Message), TipoMsg.Falla)
        End Try

    End Sub
    Public Sub CargarBeneficiarios()

        Dim oDatos As DataSet

        Try

            oDatos = New DataSet

            oDatos = Funciones.ObtenerDatos("usp_ObtenerGeneralesOP_stro")

            If Not oDatos Is Nothing AndAlso oDatos.Tables(0).Rows.Count > 0 Then

                Me.txtFechaContable.Text = oDatos.Tables(0).Rows(0).Item("FechaContable").ToString

            End If

        Catch ex As Exception
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("CargarBeneficiarios error: {0}", ex.Message), TipoMsg.Falla)
        End Try

    End Sub
    Public Sub LlenarGrid()

        Try
            grd.DataSource = oGrdOrden
            grd.DataBind()
        Catch ex As Exception
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("LlenarGrid error: {0}", ex.Message), TipoMsg.Falla)
        End Try

    End Sub
    Public Sub IniciaGrid()

        Dim dt As DataTable

        Try

            oGrdOrden = Nothing

            dt = New DataTable()

            'Campos para proveedor
            dt.Columns.Add("FolioOnbase", Type.GetType("System.String"))
            dt.Columns.Add("Factura", Type.GetType("System.String"))
            dt.Columns.Add("FechaComprobante", Type.GetType("System.String"))
            dt.Columns.Add("NumeroComprobante", Type.GetType("System.String"))

            'Campos generales
            dt.Columns.Add("IdSiniestro", Type.GetType("System.Int32"))
            dt.Columns.Add("IdPersona", Type.GetType("System.Int32"))
            dt.Columns.Add("CodigoAsegurado", Type.GetType("System.Int32"))
            dt.Columns.Add("Cod_Pres", Type.GetType("System.Int32"))
            dt.Columns.Add("Siniestro", Type.GetType("System.String"))
            dt.Columns.Add("RFC", Type.GetType("System.String"))
            dt.Columns.Add("CodigoTercero", Type.GetType("System.Int32"))
            dt.Columns.Add("Poliza", Type.GetType("System.String"))
            dt.Columns.Add("TipoMoneda", Type.GetType("System.Int32"))
            dt.Columns.Add("Moneda", Type.GetType("System.String"))
            dt.Columns.Add("CodItem", Type.GetType("System.Int32"))
            dt.Columns.Add("CodIndCob", Type.GetType("System.Int32"))
            dt.Columns.Add("SnCondusef", Type.GetType("System.Int32"))
            dt.Columns.Add("NumeroOficioCondusef", Type.GetType("System.String"))
            dt.Columns.Add("FechaOficioCondusef", Type.GetType("System.String"))
            dt.Columns.Add("ClasePago", Type.GetType("System.String"))
            dt.Columns.Add("ConceptoPago", Type.GetType("System.String"))
            dt.Columns.Add("OrigenPago", Type.GetType("System.String"))
            dt.Columns.Add("CodigoRamo", Type.GetType("System.Int32"))
            dt.Columns.Add("CodigoSubRamo", Type.GetType("System.Int32"))
            dt.Columns.Add("Pago", Type.GetType("System.Double"))
            dt.Columns.Add("Impuestos", Type.GetType("System.Double"))
            dt.Columns.Add("Retenciones", Type.GetType("System.Double"))
            dt.Columns.Add("PagoSinIva", Type.GetType("System.Double"))
            dt.Columns.Add("PagoConIva", Type.GetType("System.Double"))
            dt.Columns.Add("Descuentos", Type.GetType("System.Double"))
            dt.Columns.Add("Deducible", Type.GetType("System.Double"))
            dt.Columns.Add("TipoPago", Type.GetType("System.String"))
            dt.Columns.Add("NumeroCorrelaEstim", Type.GetType("System.Int32"))
            dt.Columns.Add("CodigoTipoStro", Type.GetType("System.Int32"))
            dt.Columns.Add("Estimacion", Type.GetType("System.Double"))
            dt.Columns.Add("ImportePagos", Type.GetType("System.Double"))
            dt.Columns.Add("Subtotal", Type.GetType("System.Double"))

            dt.Columns.Add("CondicionISR", Type.GetType("System.Int32"))
            dt.Columns.Add("CondicionCED", Type.GetType("System.Int32"))
            dt.Columns.Add("MonedaFactura", Type.GetType("System.Int32"))

            oGrdOrden = dt

            grd.DataSource = oGrdOrden
            grd.DataBind()

        Catch ex As Exception
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("IniciaGrid error: {0}", ex.Message), TipoMsg.Falla)
        End Try


    End Sub

#End Region

    Protected Sub txtSiniestro_TextChanged(sender As Object, e As EventArgs)
        txtDescripcionOP.Text = txtSiniestro.Text
    End Sub
    Protected Sub txtimporte_TextChanged(sender As Object, e As EventArgs)
        txtBeneficiario.Text = txtBeneficiario_stro.Text
    End Sub
    Public Sub LimpiarOrdenPago() Handles btnLimpiar.Click
        Dim chkdelete As CheckBox
        For Each row In grd.Rows
            chkdelete = BuscarControlPorID(row, "eliminar")
            chkdelete.Checked = True
        Next
        EliminarFila(1)

        txtOnBase.Text = ""
        txtSiniestro.Text = ""
        txtPoliza.Text = ""
        txtMonedaPoliza.Text = ""
        txtCodigoBeneficiario_stro.Text = ""
        txtBeneficiario_stro.Text = ""
        txtRFC.Text = ""
        txtTipoCambio.Text = ""

        'cmbTipoComprobante.Items.Clear()
        txtNumeroComprobante.Text = ""
        txtFechaComprobante.Text = ""

        Me.txtTotalAutorizacion.Text = "" 'importe de la poliza
        Me.txtTotalImpuestos.Text = ""
        Me.txtTotalRetenciones.Text = ""
        Me.txtTotal.Text = ""  'importe de la poliza

        Me.iptxtTotalAutorizacion.Text = "" 'importe de pago
        Me.iptxtTotalImpuestos.Text = ""
        Me.iptxtTotal.Text = ""
        'Me.iptxtTotalAutorizacionNacional.Text = ""  'importe de pago

        Me.txtcod_pres.Text = ""

        Me.txtDescripcionOP.Text = ""
        Me.txtTotalAutorizacionFac.Text = "" 'txt de facturas
        Me.txtTotalImpuestosFac.Text = ""
        Me.txtTotalRetencionesFac.Text = ""
        Me.txtTotalFac.Text = ""
        Me.txtTotalAutorizacionNacionalFac.Text = ""
        Me.txtDescuentos.Text = "" 'txt de facturas

        Me.lbldescuento.Text = ""

        Me.txtidfactura.Text = ""

        Me.txtCodigoCuenta.Text = ""
        Me.txtDescCuenta.Text = ""

        Me.oSucursalT_stro.Value = String.Empty
        Me.oBancoT_stro.Value = String.Empty
        Me.oBeneficiarioT_stro.Value = String.Empty
        Me.oCuentaBancariaT_stro.Value = String.Empty
        Me.oMonedaT_stro.Value = String.Empty
        Me.oTipoCuentaT_stro.Value = String.Empty
        Me.oPlazaT_stro.Value = String.Empty
        Me.oAbaT_stro.Value = String.Empty

        txtBeneficiario.Text = ""
        'cmbOrigendePago.Items.Clear()
        txtBeneficiario.Text = ""
        cmbTipoUsuario.Enabled = True


    End Sub
    Public Sub txtOnImporteTerAseg(sender As Object, e As EventArgs)
        Me.txtTotalAutorizacion.Text = txtimporteTerAseg.Text 'importe de la poliza
        Me.txtTotalImpuestos.Text = 0
        Me.txtTotalRetenciones.Text = 0
        Me.txtTotal.Text = txtimporteTerAseg.Text  'importe de la poliza

        Me.iptxtTotalAutorizacion.Text = txtimporteTerAseg.Text 'importe de pago
        Me.iptxtTotalImpuestos.Text = 0
        Me.iptxtTotalRetenciones.Text = 0
        Me.iptxtTotal.Text = txtimporteTerAseg.Text
        Me.iptxtTotalNacional.Text = txtimporteTerAseg.Text
    End Sub
    Protected Sub txtOnBase_TextChanged(sender As Object, e As EventArgs)
        Try
            '--Inicio Esto es para limpiar el grid
            Dim chkdelete As CheckBox
            For Each row In grd.Rows
                chkdelete = BuscarControlPorID(row, "eliminar")
                chkdelete.Checked = True
            Next
            EliminarFila(1)
            '--FIN Esto es para limpiar el grid
            Dim oDatos As DataSet
            oDatos = New DataSet
            If txtOnBase.Text <> "" Then
                Dim oParametros As New Dictionary(Of String, Object)
                oParametros.Add("Accion", "2")
                oParametros.Add("Folio_OnBase", txtOnBase.Text.Trim())
                oDatos = Funciones.ObtenerDatos("MIS_sp_cir_op_stro_Catalogos_Fondos", oParametros)
                If (oDatos.Tables(0).Rows(0).Item("Mensaje") = "1") Then
                    If (oDatos.Tables(0).Rows(0).Item("sn_relacionado") = "-1") Then
                        Mensaje.MuestraMensaje("Folio OnBase Relacionado", "Fecha Relacionado: " + oDatos.Tables(0).Rows(0).Item("fecha_relacion").ToString() + " Usuario relacion: " + oDatos.Tables(0).Rows(0).Item("cod_usuario_relacion").ToString() + " Fecha de Comprobante: " + oDatos.Tables(0).Rows(0).Item("fecha_emision_gmx").ToString(), TipoMsg.Falla)
                    Else
                        With oDatos.Tables(0).Rows(0)
                            Me.txtSiniestro.Text = .Item("num_siniestro")
                            'Me.txtPoliza.Text = .Item("txt_denomin")
                            'Me.txtMonedaPoliza.Text = .Item("cod_cta_cble")
                            'Me.cmbConcepto.Text = .Item("txt_denomin")
                            Me.txtcod_pres.Text = .Item("cod_pres")
                            Me.txtidfactura.Text = .Item("id_factura")
                            Me.txtNumeroComprobante.Text = .Item("folio_GMX")
                            Me.txtFechaComprobante.Text = .Item("fecha_emision_gmx")
                            Me.txtCodigoBeneficiario_stro.Text = .Item("id_persona")
                            Me.txtBeneficiario_stro.Text = .Item("NOMBRE_PROVEEDOR")
                            Me.txtRFC.Text = .Item("RFC_proveedor")
                            Me.txtTotalFac.Text = .Item("imp_total")
                            Me.txtTotalAutorizacionFac.Text = .Item("imp_subtotal")
                            Me.txtTotalAutorizacionNacionalFac.Text = .Item("imp_subtotal")
                            Me.txtTotalImpuestosFac.Text = .Item("imp_impuestos")
                            Me.txtTotalRetencionesFac.Text = .Item("imp_retencion")
                            Me.txtBeneficiario.Text = .Item("NOMBRE_PROVEEDOR")
                            Me.txtDescripcionOP.Text = .Item("num_siniestro").ToString()
                        End With
                    End If
                    'Me.cmbconceptoProveedor.Items.Clear()
                    'For Each fila In oDatos.Tables(1).Rows
                    '    Me.cmbconceptoProveedor.Items.Add(New ListItem(String.Format(fila.Item("cod_cpto")).ToUpper + " || " + fila.Item("txt_desc") + " || " + String.Format(fila.Item("cod_clase_pago")).ToUpper, fila.Item("cod_cpto")))
                    'Next
                    Me.cmbConcepto.Items.Clear()
                    For Each fila In oDatos.Tables(1).Rows
                        Me.cmbConcepto.Items.Add(New ListItem(String.Format(fila.Item("cod_cpto")).ToUpper + " || " + fila.Item("txt_desc") + " || " + String.Format(fila.Item("cod_clase_pago")).ToUpper, fila.Item("cod_cpto")))
                    Next
                Else
                    Mensaje.MuestraMensaje("Fecha Comprobante menor al año fiscal: ", "Fecha de Emision Fiscal" + oDatos.Tables(0).Rows(0).Item("fecha_emision_gmx").ToString(), TipoMsg.Falla)
                End If
            End If
        Catch ex As Exception
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("grd_RowDataBound error: {0}", ex.Message), TipoMsg.Falla)
        End Try
    End Sub
End Class
