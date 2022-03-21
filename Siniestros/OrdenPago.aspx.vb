Imports System.Data
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
        Ninguno = 1 'VZAVALETA_10290_INICIO
        Asegurado = 7
        Tercero = 8
        Proveedor = 10
    End Enum

    Dim Salir As Integer 'VZAVALETA_10290_INICIO

#End Region

#Region "Eventos"
    Sub Page_Load(ByVal Sender As Object, ByVal e As EventArgs) Handles Me.Load

        If Not IsPostBack Then
            Master.Titulo = "OP Tradicional"
            InicializarValores()
            linkOnBase.HRef = "" 'FJCP 12090 MEJORAS Folio OnBase
            'FJCP_10290_CC INI
            hidBloqueoOnbase.Value = 1
            Master.InformacionGeneral()
        Else
            hidBloqueoOnbase.Value = 0
        End If

        hidCodUsuario.Value = Master.cod_usuario
        'FJCP_10290_CC FIN

        If cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then
            'Onbase.Style("display") = ""
            pnlProveedor.Style("display") = ""
            Facturas0.Style("display") = ""
            Facturas1.Style("display") = ""
            cmbOrigenOP.Enabled = False
            cmbTipoPagoOP.Enabled = False
            txtRFC.Enabled = False
            txtBeneficiario_stro.Enabled = False
            cmbTipoComprobante.Enabled = False
            txtNumeroComprobante.Enabled = False
            txtFechaComprobante.Enabled = False
            Me.btnVerCuentas.Visible = True
            'cmbTipoComprobante.Items.Clear()

            chkVariosConceptos.Visible = True 'FJCP MULTIPAGO 
        Else
            'Onbase.Style("display") = "" 'FFUENTES
            'pnlProveedor.Style("display") = "none"  
            pnlProveedor.Style("display") = ""  'FJCP MULTIPAGO 
            Facturas0.Style("display") = "none"
            Facturas1.Style("display") = "none"
            cmbOrigenOP.Enabled = False
            cmbTipoPagoOP.Enabled = True
            cmbTipoComprobante.Enabled = False
            txtNumeroComprobante.Enabled = True
            txtFechaComprobante.Enabled = True
            txtRFC.Enabled = True
            txtBeneficiario_stro.Enabled = True
            Me.btnVerCuentas.Visible = True
            chkVariosConceptos.Visible = False
            'lblDependencias.Visible = False
            'drDependencias.Visible = False
        End If



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

                txtConceptoOP.Text = String.Empty
                Dim substroDetalle As Integer = 0

                If Not oDatos Is Nothing AndAlso oDatos.Rows.Count > 0 Then

                    For Each oFila In oDatos.Rows

                        If txtConceptoOP.Text.Trim = String.Empty Then
                            'txtConceptoOP.Text = String.Format("{0} {1}", txtConceptoOP.Text.Trim, oFila("Siniestro"))
                            txtConceptoOP.Text = String.Format("{0} {1} {2}", txtConceptoOP.Text.Trim, oFila("Siniestro"), oFila("Subsiniestro")) 'FJCP MEJORAS 10290 DETALLES
                            substroDetalle = CInt(oFila("Subsiniestro"))
                        Else
                            'txtConceptoOP.Text = String.Format("{0}, {1}", txtConceptoOP.Text.Trim, oFila("Siniestro"))
                            If substroDetalle <> CInt(oFila("Subsiniestro")) Then
                                txtConceptoOP.Text = String.Format("{0}, {1}", txtConceptoOP.Text.Trim, oFila("Subsiniestro")) 'FJCP MEJORAS 10290 DETALLES
                            End If
                        End If
                    Next

                    txtConceptoOP.Text = String.Format("{0} {1}", txtConceptoOP.Text.Trim, oClavesPago.Select(String.Format("cod_clase_pago = '{0}'", oDatos.Rows(0)("ClasePago")))(0)("txt_desc"))

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
                    limpiarCamposTipoUsuario() 'FJCP 10290 MEJORAS Pagar A
                    habilitarCampos() 'FJCP 10290 MEJORAS Pagar A
                    'Me.txtBeneficiario.Text = String.Empty
                    'Me.txtOnBase.Text = String.Empty
                    'Me.txtSiniestro.Text = String.Empty
                    'Me.txtPoliza.Text = String.Empty
                    'Me.txtMonedaPoliza.Text = String.Empty
                    'Me.txtRFC.Text = String.Empty
                    'Me.txtCodigoBeneficiario_stro.Text = String.Empty
                    'Me.txtBeneficiario_stro.Text = String.Empty

                    'Me.txtTotalAutorizacion.Text = String.Empty 'importe de la poliza
                    'Me.txtTotalImpuestos.Text = String.Empty
                    'Me.txtTotalRetenciones.Text = String.Empty
                    'Me.txtTotal.Text = String.Empty  'importe de la poliza

                    'Me.iptxtTotalAutorizacion.Text = String.Empty 'importe de pago
                    'Me.iptxtTotalImpuestos.Text = String.Empty
                    'Me.iptxtTotal.Text = String.Empty  'importe de pago

                    'Me.txtTotalAutorizacionFac.Text = String.Empty 'txt de facturas
                    'Me.txtTotalImpuestosFac.Text = String.Empty
                    'Me.txtTotalRetencionesFac.Text = String.Empty
                    'Me.txtTotalFac.Text = String.Empty
                    'Me.txtTotalAutorizacionNacionalFac.Text = String.Empty
                    'Me.txtDescuentos.Text = String.Empty 'txt de facturas

                    'Me.txtConceptoOP.Text = String.Empty
                    'Me.oSucursalT_stro.Value = String.Empty
                    'Me.oBancoT_stro.Value = String.Empty
                    'Me.oBeneficiarioT_stro.Value = String.Empty
                    'Me.oCuentaBancariaT_stro.Value = String.Empty
                    'Me.oMonedaT_stro.Value = String.Empty
                    'Me.oTipoCuentaT_stro.Value = String.Empty
                    'Me.oPlazaT_stro.Value = String.Empty
                    'Me.oAbaT_stro.Value = String.Empty



                    'If Me.cmbSubsiniestro.Items.Count > 0 Then
                    '    Me.cmbSubsiniestro.Items.Clear()
                    'End If

                    'If cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then
                    '    Me.cmbTipoPagoOP.SelectedValue = "T"
                    '    Me.btnVerCuentas.Visible = True
                    '    Me.txtSiniestro.Enabled = False
                    'Else
                    '    Me.cmbTipoPagoOP.SelectedValue = "T"
                    '    Me.btnVerCuentas.Visible = False
                    '    Me.txtSiniestro.Enabled = True
                    'End If


                Case "tipo_pago_OP"

                    'Me.btnVerCuentas.Visible = IIf(cmbTipoPagoOP.SelectedValue = "C", False, True)
                    If cmbTipoPagoOP.SelectedValue = "T" Then
                        Me.btnVerCuentas.Visible = True
                    Else
                        Me.btnVerCuentas.Visible = False
                    End If

                Case "clase_pago"

                    'If cmb.SelectedValue = "26" Then
                    '    If Not SiniestroAbierto(CLng(oGrdOrden.Rows(iFila)("Siniestro")), CInt(oGrdOrden.Rows(iFila)("Subsiniestro"))) Then
                    '        Mensaje.MuestraMensaje("Orden de pago de siniestros", String.Format("EL SINIESTRO {0} - {1} ESTA CERRADO O CANCELADO. SE TOMARA LA SIGUIENTE CLASE DE PAGO DISPONIBLE", oGrdOrden.Rows(iFila)("Siniestro"), oGrdOrden.Rows(iFila)("Subsiniestro")), TipoMsg.Advertencia)
                    '        oGrdOrden.Rows(iFila)("ClasePago") = oClavesPago.Rows(1).Item(0)
                    '        cmb.SelectedValue = oClavesPago.Rows(1).Item(0)
                    '    End If
                    '    cmbOrigenOP.SelectedValue = 5
                    'End If
                    'If cmb.SelectedValue = "27" Then
                    '    oGrdOrden.Rows(iFila)("ConceptoPago") = IIf(oGrdOrden.Rows(iFila)("ClasePago") = "27", "350", String.Empty)
                    '    cmbOrigenOP.SelectedValue = 6
                    'End If
                    'oGrdOrden.Rows(iFila)("ClasePago") = cmb.SelectedValue

                    ''Selección automática en INDEMINIZACIONES
                    'oGrdOrden.Rows(iFila)("ConceptoPago") = IIf(oGrdOrden.Rows(iFila)("ClasePago") = "26", "350", String.Empty)

                    'CargarConceptosPagodefault(row, iFila, cmb.SelectedValue, "0")

                    ''En caso de que el siniestro haya sido cancelado y se haya asignado la clase de pago
                    ''de honorarios o gastos de siniestros se seleccionara el tipo de pago como final,
                    ''de lo contrario se habilitara para poder cambiar el tipo de pago
                    'If cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor AndAlso
                    '        (oGrdOrden.Rows(iFila)("ClasePago") = "27" OrElse oGrdOrden.Rows(iFila)("ClasePago") = "28") Then

                    '    cmb = New DropDownList
                    '    cmb = BuscarControlPorClase(row, "estandar-control tipo_pago")

                    '    If Not cmb Is Nothing Then
                    '        cmb.SelectedValue = "F"
                    '        cmb.Enabled = False
                    '    End If

                    '    oGrdOrden.Rows(iFila)("TipoPago") = 2

                    'Else

                    '    cmb = New DropDownList
                    '    cmb = BuscarControlPorClase(row, "estandar-control tipo_pago")

                    '    If Not cmb Is Nothing Then
                    '        cmb.Enabled = True
                    '    End If

                    'End If

                    'CalcularTotales()

                Case "concepto_pago"

                    Dim cmbClasePago As DropDownList
                    cmbClasePago = New DropDownList


                    If cmb.SelectedValue = 350 Or cmb.SelectedValue = 365 Then
                        If Not SiniestroAbierto(CLng(oGrdOrden.Rows(iFila)("Siniestro")), CInt(oGrdOrden.Rows(iFila)("Subsiniestro"))) Then
                            Mensaje.MuestraMensaje("Orden de pago de siniestros", String.Format("EL SINIESTRO {0} - {1} ESTA CERRADO O CANCELADO. SE TOMARA LA SIGUIENTE CLASE DE PAGO DISPONIBLE", oGrdOrden.Rows(iFila)("Siniestro"), oGrdOrden.Rows(iFila)("Subsiniestro")), TipoMsg.Advertencia)
                            oGrdOrden.Rows(iFila)("ClasePago") = oClavesPago.Rows(1).Item(0)
                            cmb.SelectedValue = 905
                            cmbOrigenOP.SelectedValue = 5
                        End If
                    End If
                    'FJCP Multipago- Se comenta clase de pago Ini 
                    If chkVariasFacturas.Checked = False Then
                        CargarClasePago(row, iFila, txtCodigoBeneficiario_stro.Text, cmb.SelectedValue)
                    End If
                    'FJCP Multipago- Se comenta clase de pago Fin
                    cmbClasePago = BuscarControlPorClase(row, "estandar-control clase_pago")
                    cmbOrigenOP.SelectedValue = 6
                    If (cmb.SelectedValue <> 350 Or cmb.SelectedValue <> 1260) Then
                        oGrdOrden.Rows(iFila)("ConceptoPago") = cmb.SelectedValue
                        oGrdOrden.Rows(iFila)("ClasePago") = cmbClasePago.SelectedValue
                        If cmbClasePago.SelectedValue = 26 Or cmbClasePago.SelectedValue = 75 Then
                            cmbOrigenOP.SelectedValue = 5
                        Else
                            cmbOrigenOP.SelectedValue = 6
                        End If
                        CalcularTotales()
                    End If
                    If cmbClasePago.SelectedValue = 26 Or cmbClasePago.SelectedValue = 75 Then
                        cmbOrigenOP.SelectedValue = 5
                    Else
                        cmbOrigenOP.SelectedValue = 6
                    End If
                Case "tipo_pago"
                    oGrdOrden.Rows(iFila)("TipoPago") = IIf(cmb.SelectedValue = "P", 1, 2)
            End Select



        Catch ex As Exception
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("cmb_SelectedIndexChanged error: {0}", ex.Message), TipoMsg.Falla)
        End Try

    End Sub
    Protected Sub grd_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)

        Dim oSelector As DropDownList
        Dim oSelectorcpto As DropDownList
        Dim oSelectorpago As DropDownList

        Dim iIndex As Integer

        Try

            If e.Row.RowType = DataControlRowType.DataRow Then

                iIndex = e.Row.RowIndex

                'Clase de pago
                oSelector = New DropDownList
                oSelectorcpto = New DropDownList
                oSelectorpago = New DropDownList

                oSelector = BuscarControlPorClase(e.Row, "estandar-control clase_pago")
                oSelectorcpto = BuscarControlPorClase(e.Row, "estandar-control concepto_pago")
                oSelectorpago = BuscarControlPorClase(e.Row, "estandar-control pago")

                oSelector.DataSource = oClavesPago
                oSelector.DataTextField = "txt_desc"
                oSelector.DataValueField = "cod_clase_pago"
                oSelector.DataBind()
                oSelector.SelectedValue = oGrdOrden.Rows(iIndex)("ClasePago") 'JLCORTES

                Dim cerrado_open_stro As Int16 = 0


                If oGrdOrden.Rows(iIndex)("ClasePago") = "26" Then
                    If Not SiniestroAbierto(CLng(Me.txtSiniestro.Text.Trim), CInt(Me.cmbSubsiniestro.SelectedValue)) Then
                        If cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then
                            Mensaje.MuestraMensaje("Orden de pago de siniestros", String.Format("EL SINIESTRO {0} - {1} ESTA CERRADO O CANCELADO. SE TOMARA LA SIGUIENTE OPCION DE CLASE DE PAGO ", Me.txtSiniestro.Text.Trim, Me.cmbSubsiniestro.SelectedItem.Text), TipoMsg.Advertencia)
                            oGrdOrden.Rows(iIndex)("ClasePago") = "27"
                            cmbOrigenOP.SelectedValue = 6
                            cerrado_open_stro = 1
                        Else
                            Mensaje.MuestraMensaje("Orden de pago de siniestros", String.Format("EL SINIESTRO {0} - {1} ESTA CERRADO O CANCELADO. NO SE PUEDE REALIZAR UN PAGO", Me.txtSiniestro.Text.Trim, Me.cmbSubsiniestro.SelectedItem.Text), TipoMsg.Advertencia)
                            oSelectorcpto.Items.Clear()
                            oSelector.Items.Clear()
                            cmbOrigenOP.Items.Clear()
                            cerrado_open_stro = 1
                        End If
                    End If
                End If

                'oSelector.Items.FindByValue(oGrdOrden.Rows(iIndex)("ClasePago")).Selected = True

                'Concepto de pago
                If cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then
                    CargarConceptosPagodefault(e.Row, iIndex, oGrdOrden.Rows(iIndex)("ClasePago"), cerrado_open_stro)

                    'FJCP Multipago - Se comenta clase de pago Ini
                    If chkVariasFacturas.Checked = False Then
                        oGrdOrden.Rows(iIndex)("ClasePago") = oSelector.SelectedValue
                    End If
                    'FJCP Multipago- Fin
                    oGrdOrden.Rows(iIndex)("ConceptoPago") = oSelectorcpto.SelectedValue
                    'carga la clase de pago
                    'FJCP Multipago- Se comenta clase de pago Ini Se comenta cargar clase de pago
                    If chkVariasFacturas.Checked = False Then
                        CargarClasePago(e.Row, iIndex, txtCodigoBeneficiario_stro.Text, oSelectorcpto.SelectedValue)
                    End If
                    'FJCP Multipago - Se comenta clase de pago Ini
                    'Dim clase_pago_default As Int16 = oSelector.SelectedValue
                    'Dim cpto_default As Int16 = oSelectorcpto.SelectedValue



                Else
                    If cerrado_open_stro = 0 Then
                        CargarConceptosPagodefault(e.Row, iIndex, oGrdOrden.Rows(iIndex)("ClasePago"), cerrado_open_stro)
                        oGrdOrden.Rows(iIndex)("ClasePago") = oSelector.SelectedValue
                        oGrdOrden.Rows(iIndex)("ConceptoPago") = oSelectorcpto.SelectedValue
                    Else
                        oGrdOrden.Rows(iIndex)("ClasePago") = ""
                        oGrdOrden.Rows(iIndex)("ConceptoPago") = ""
                    End If
                End If


                'En caso de que el siniestro haya sido cancelado y se haya asignado la clase de pago
                'de honorarios o gastos de siniestros se seleccionara el tipo de pago como final,
                'de lo contrario se habilitara para poder cambiar el tipo de pago
                If cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor AndAlso
                        (oGrdOrden.Rows(iIndex)("ClasePago") = "27" OrElse oGrdOrden.Rows(iIndex)("ClasePago") = "28") Then

                    oSelector = New DropDownList
                    oSelector = BuscarControlPorClase(e.Row, "estandar-control tipo_pago")

                    If Not oSelector Is Nothing Then
                        'oSelector.SelectedValue = "F"'Se comento para cargue por default el tipo de pago
                        'oSelector.Enabled = False' ffuentes 20191120 se agrega este codigo por que aun sea final siempre puede cambiar
                        oSelector.SelectedValue = oSeleccionActual.Rows(0).Item("Id_Pagos").ToString() 'se agrego para cargar desde la tabla intermedia si es parcial/final aseurados terceros 
                        oSelector.Enabled = True
                        If oSeleccionActual.Rows(0).Item("Id_Pagos").ToString() = " " Then
                            oGrdOrden.Rows(iIndex)("TipoPago") = 1
                        End If
                        If oSeleccionActual.Rows(0).Item("Id_Pagos").ToString() = "P" Then
                            oGrdOrden.Rows(iIndex)("TipoPago") = 1
                        Else
                            oGrdOrden.Rows(iIndex)("TipoPago") = 2
                        End If
                    End If
                    If cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then
                        If oGrdOrden.Rows(iIndex)("ClasePago") <> "26" Then
                            cmbOrigenOP.SelectedValue = 6
                        End If
                    End If
                Else

                    oSelector = New DropDownList
                    oSelector = BuscarControlPorClase(e.Row, "estandar-control tipo_pago")

                    If Not oSelector Is Nothing Then
                        oSelector.SelectedValue = oSeleccionActual.Rows(0).Item("Id_Pagos").ToString() 'se agrego para cargar desde la tabla intermedia si es parcial/final aseurados terceros 
                        oSelector.Enabled = True
                        If oSeleccionActual.Rows(0).Item("Id_Pagos").ToString() = "P" Then
                            oGrdOrden.Rows(iIndex)("TipoPago") = 1
                        Else
                            oGrdOrden.Rows(iIndex)("TipoPago") = 2
                        End If
                    End If
                End If
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

            If Not String.IsNullOrWhiteSpace(Me.txtFechaContable.Text.Trim) AndAlso IsDate(Me.txtFechaContable.Text.Trim) Then
                ObtenerTipoCambio()
            Else
                Mensaje.MuestraMensaje("OrdenPagoSiniestros", "Ingrese una fecha válida", TipoMsg.Advertencia)
                Return
            End If

            oDatos = New DataSet
            oDataTable = New DataTable

            oTxt = New TextBox

            oTxt = sender

            sElemento = oTxt.CssClass.Substring(oTxt.CssClass.IndexOf(" "c) + 1)
            sElemento = sElemento.Substring(0, sElemento.IndexOf(" "c))

            Select Case sElemento

                Case "onbase"

                    'si tiene más de un pago no se manda llamar el método solamente se carga combo num pago 

                    If validaNumeroPago() = False Then 'FJCP MEJORAS FASE II NUMERO PAGO
                        Exit Sub
                        'Else
                        '    MuestraMensaje("Titulo", "correcto", TipoMsg.Advertencia)
                    End If



                Case "siniestro"

                    EliminarFila(1)
                    Select Case Me.cmbTipoUsuario.SelectedValue
                        Case eTipoUsuario.Asegurado, eTipoUsuario.Tercero

                            oParametros.Add("Numero_Siniestro", Me.txtSiniestro.Text.Trim)

                            oDatos = Funciones.ObtenerDatos("sp_op_stro_consulta_numero_siniestro", oParametros)

                            Me.cmbSubsiniestro.Items.Clear()
                            Me.cmbOrigenOP.Items.Clear()

                            If Not oDatos Is Nothing AndAlso oDatos.Tables(0).Rows.Count > 0 Then

                                oSeleccionActual = oDatos.Tables(0)

                                With oDatos.Tables(0).Rows(0)
                                    Me.txtSiniestro.Text = .Item("nro_stro")
                                    Me.txtRFC.Text = IIf(Me.cmbTipoUsuario.SelectedValue = eTipoUsuario.Asegurado, .Item("RFC"), String.Empty)
                                    Me.txtPoliza.Text = .Item("poliza")
                                    Me.txtMonedaPoliza.Text = .Item("txt_desc")
                                    Me.txtBeneficiario.Text = String.Format("{0} {1} {2}", .Item("txt_apellido1"), .Item("txt_apellido2"), .Item("txt_nombre")).ToUpper

                                    If Me.cmbTipoUsuario.SelectedValue = eTipoUsuario.Asegurado Then
                                        Me.txtBeneficiario_stro.Text = Me.txtBeneficiario.Text.Trim
                                        Me.txtCodigoBeneficiario_stro.Text = .Item("cod_aseg")
                                        'FJCP 10290 MEJORAS Nombre y Razón Social - Asegurado
                                        LlenaDDLAsegurado(Me.txtPoliza.Text.Trim)
                                        txtBeneficiario_stro.Visible = False
                                    Else
                                        Me.txtBeneficiario_stro.Text = String.Empty
                                        Me.txtCodigoBeneficiario_stro.Text = String.Empty
                                        Me.txtBeneficiario.Text = String.Empty
                                    End If
                                    'se agrego por el tema de los 4 campos mas 
                                    If .Item("Forma_Hara_Pago") = -1 Then
                                        Me.cmbTipoPagoOP.SelectedValue = "T"
                                    Else
                                        If .Item("Forma_Hara_Pago") = 0 Then
                                            Me.cmbTipoPagoOP.SelectedValue = "C"
                                        End If
                                    End If

                                End With

                                oClavesPago = IIf(oDatos.Tables(1) Is Nothing OrElse oDatos.Tables(1).Rows.Count = 0, Nothing, oDatos.Tables(1))

                                If Not oDatos.Tables(2) Is Nothing AndAlso oDatos.Tables(2).Rows.Count > 0 Then

                                    oOrigenesPago = IIf(oOrigenesPago Is Nothing OrElse oOrigenesPago.Rows.Count = 0, oDatos.Tables(2), oOrigenesPago)

                                    If Me.cmbOrigenOP.Items.Count > 0 Then
                                        Me.cmbOrigenOP.Items.Clear()
                                    End If

                                    For Each fila In oDatos.Tables(2).Rows
                                        Me.cmbOrigenOP.Items.Add(New ListItem(fila.Item("DescripcionOrigenPago").ToString.ToUpper, fila.Item("CodigoOrigenPago")))
                                    Next

                                End If

                                For Each fila In oDatos.Tables(0).Rows
                                    Me.cmbSubsiniestro.Items.Add(New ListItem(String.Format("Subsiniestro {0}", fila.Item("id_substro")).ToUpper, fila.Item("id_substro")))
                                Next


                                'CARGO LOS TIPOS DE CODUMENTOS PARA ASEGURADOS Y TERCEROS
                                cmbTipoComprobante.Items.Clear()
                                If cmbTipoComprobante.Items.Count = 0 Then

                                    cmbTipoComprobante.DataSource = oDatos.Tables(4)
                                    cmbTipoComprobante.DataTextField = "Descripcion_Doc"
                                    cmbTipoComprobante.DataValueField = "Id_Tipo_Doc"
                                    cmbTipoComprobante.DataBind()

                                    cmbTipoComprobante.SelectedValue = oDatos.Tables(0).Rows(0).Item("Id_Tipo_Doc")
                                End If

                                Me.txtTipoCambio.Text = IIf(Me.txtMonedaPoliza.Text = "NACIONAL", "1.00", ObtenerTipoCambio.ToString())

                            Else

                                oSeleccionActual = Nothing

                                Me.txtOnBase.Text = String.Empty
                                Me.txtSiniestro.Text = String.Empty
                                Me.txtRFC.Text = String.Empty
                                Me.txtPoliza.Text = String.Empty
                                Me.txtMonedaPoliza.Text = String.Empty
                            End If

                            'Onbase.Style("display") = "none" 'FFUENTES none
                            'pnlProveedor.Style("display") = "none"
                            pnlProveedor.Style("display") = "" 'FJCP MULTIPAGO

                        Case Else

                    End Select

                Case "fechaContable"
                    If Not String.IsNullOrWhiteSpace(Me.txtFechaContable.Text.Trim) AndAlso IsDate(Me.txtFechaContable.Text.Trim) Then
                        ObtenerTipoCambio()
                    Else
                        Mensaje.MuestraMensaje("OrdenPagoSiniestros", "Ingrese una fecha válida", TipoMsg.Advertencia)
                    End If
                Case "fechadepago"
                    'MMQ se corrigio validacion de fechas
                    If Convert.ToDateTime(Me.txtFechaEstimadaPago.Text) < Now.ToShortDateString Then
                        Mensaje.MuestraMensaje("OrdenPagoSiniestros", "No Puede ingresar una fecha menor al dia de hoy", TipoMsg.Advertencia)
                        Me.txtFechaEstimadaPago.Text = Now.ToString("dd/MM/yyyy")
                    End If

                Case "RFC"
                    ObtenerRFC("0", "2", txtRFC.Text)
                Case "Nombre"
                    ObtenerRFC("0", "2", txtRFC.Text)
            End Select


        Catch ex As Exception
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("txt_TextChanged error: {0}", ex.Message), TipoMsg.Falla)

            oSeleccionActual = Nothing

            Me.txtOnBase.Text = String.Empty
            Me.txtSiniestro.Text = String.Empty
            Me.txtRFC.Text = String.Empty
            Me.txtPoliza.Text = String.Empty
            Me.txtMonedaPoliza.Text = String.Empty

            'Me.lblObBase.Visible = False
            'Me.txtOnBase.Visible = False 'FFUENTES false

        End Try

    End Sub
    Public Sub grid_TextChanged(sender As Object, e As EventArgs)

        Dim pagoenmonedanacional As Decimal
        Dim oTxt As TextBox
        'Dim oTxtDescuento As TextBox
        Dim oFila As GridViewRow

        Dim iIndiceFila As Integer

        Dim sElemento As String = String.Empty

        Try

            oTxt = New TextBox
            'oTxtDescuento = New TextBox

            oTxt = sender
            'oTxtDescuento = sender
            oFila = oTxt.NamingContainer
            'oFila = oTxtDescuento.NamingContainer
            iIndiceFila = oFila.RowIndex

            sElemento = oTxt.CssClass.Substring(oTxt.CssClass.IndexOf(" "c) + 1)
            'sElemento = oTxtDescuento.CssClass.Substring(oTxtDescuento.CssClass.IndexOf(" "c) + 1)

            Select Case sElemento

                Case "pago"
                    'JLC Mejoras -Inicio
                    oTxt.Text.Replace(",", "")
                    'JLC Mejoras -fin

                    If IsNumeric(oTxt.Text.Trim) Then
                        'SE AGREGO PARA VARIOS CONCEPTOS Y PARA QUE PUEDA HACER EL CALCULO
                        'If chkVariosConceptos.Checked = True Then
                        If chkVariosConceptos.Checked = True Or chkVariasFacturas.Checked = True Then
                            oGrdOrden.Columns("Pago").ReadOnly = False
                            oGrdOrden.Rows(iIndiceFila)("Pago") = CDbl(oTxt.Text.Trim)
                        End If
                        pagoenmonedanacional = oTxt.Text
                        If txtMonedaPoliza.Text = "DOLAR AMERICANO" Then
                            If cmbMonedaPago.SelectedValue = "0" Then
                                oTxt.Text = CDbl(oTxt.Text) / CDbl(txtTipoCambio.Text)
                            End If
                        End If
                        'If Not cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor AndAlso CDbl(oGrdOrden.Rows(iIndiceFila)("ImportePagos")) + CDbl(oTxt.Text.Trim) > CDbl(oGrdOrden.Rows(iIndiceFila)("Reserva")) Then
                        If Not cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor AndAlso CDbl(oTxt.Text.Trim) > CDbl(oGrdOrden.Rows(iIndiceFila)("Reserva")) AndAlso oGrdOrden.Rows(iIndiceFila)("ClasePago") <> 75 Then
                            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("Límite de Reserva superado. {0} Reserva: {1} {2} Total de pagos: {3}",
                                                                                                Environment.NewLine,
                                                                                                CDbl(oGrdOrden.Rows(iIndiceFila)("Reserva")),
                                                                                                Environment.NewLine,
                                                                                                CDbl(oGrdOrden.Rows(iIndiceFila)("ImportePagos"))), TipoMsg.Advertencia)
                            limpiarCamposImporte() 'VZAVALETA_10290_INCIDENCIAS
                        Else
                            oTxt.Text = pagoenmonedanacional
                            If oGrdOrden.Columns("Pago").ReadOnly Then
                                oTxt.Text = oGrdOrden.Rows(iIndiceFila)("Pago")
                            Else
                                oGrdOrden.Rows(iIndiceFila)("Pago") = Math.Round(CDbl(oTxt.Text.Trim), 2)
                                CalcularTotales()
                            End If
                        End If
                        'FJCP 10290 MEJORAS Pago solicitado excede a la cobertura de la póliza
                        If validaPagoVSmontoCob(oGrdOrden.Rows(iIndiceFila)("Siniestro").ToString(), oGrdOrden.Rows(iIndiceFila)("Subsiniestro").ToString(), CDbl(oTxt.Text.Trim)) Then
                            MuestraMensaje("Alerta de Pago", "El importe de pago solicitado excede al monto de la cobertura de la póliza", TipoMsg.Advertencia)

                        End If

                    Else
                        oTxt.Text = IIf(IsDBNull(oGrdOrden.Rows(iIndiceFila)("Pago")), "", oGrdOrden.Rows(iIndiceFila)("Pago"))
                    End If
                    'JLC Mejoras -Inicio
                    oTxt.Text = Format(CDbl(oTxt.Text), "##,##0.00")
                    Funciones.EjecutaFuncion("FormatCurrency(" + iIndiceFila.ToString() + ")", "Formato")
            'JLC Mejoras -fin

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
        Dim oFilaSeleccion2() As DataRow
        Dim oFilaSeleccion3() As DataRow

        Dim substroDetalle As Integer = 0
        Me.txtcpto2.Text = String.Empty
        Try

            If cmbNumPago.SelectedValue = -1 Then
                Mensaje.MuestraMensaje("OrdenPagoSiniestros", "Numero de Pago no seleccionado", TipoMsg.Advertencia)
                Return
            End If



            oTabla = New DataTable()

            If Me.txtCodigoBeneficiario_stro.Text = String.Empty OrElse Me.txtBeneficiario_stro.Text.Trim = String.Empty Then
                Me.txtBeneficiario_stro.Text = String.Empty
                Me.txtCodigoBeneficiario_stro.Text = String.Empty
                Mensaje.MuestraMensaje("OrdenPagoSiniestros", "Nombre o razón social no definido", TipoMsg.Advertencia)
                Return
            End If

            If cmbTipoUsuario.SelectedValue = eTipoUsuario.Tercero Then
                If Not ObtenerRFC(Me.txtCodigoBeneficiario_stro.Text, "1", "0") Then
                    Mensaje.MuestraMensaje("OrdenPagoSiniestros", "RFC Inexistente", TipoMsg.Advertencia)
                    Return
                End If
            End If

            If cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then

                If Me.txtFechaComprobante.Text.Trim = String.Empty Then
                    Mensaje.MuestraMensaje("OrdenPagoSiniestros", "No se ha indicando la fecha del comprobante", TipoMsg.Advertencia)
                    Return
                End If

                If Not ValidarFechaComprobante(Me.txtFechaComprobante.Text.Trim, CInt(Me.txtSiniestro.Text.Trim)) Then
                    Mensaje.MuestraMensaje("OrdenPagoSiniestros", "Fecha de comprobante, menor a fecha ocurrencia stro(s)", TipoMsg.Advertencia)
                    Return
                End If

            End If

            If oGrdOrden IsNot Nothing Then

                Dim oRegistro = Nothing

                Select Case cmbTipoUsuario.SelectedValue

                    Case eTipoUsuario.Asegurado, eTipoUsuario.Tercero
                        If String.IsNullOrWhiteSpace(txtSiniestro.Text) OrElse cmbSubsiniestro.Items.Count = 0 Then
                            Mensaje.MuestraMensaje("OrdenPagoSiniestros", "Siniestro y/o subsiniestro vacío", TipoMsg.Falla)
                            Return
                        End If


                        If chkVariasFacturas.Checked = False Then 'FJCP MULTIPAGO  SE AGREGA  Key .RFC = x.Field(Of String)("RFC"),
                            oRegistro = oGrdOrden.AsEnumerable().[Select](Function(x) New With {
                                        Key .Siniestro = x.Field(Of String)("Siniestro"),
                                         Key .RFC = x.Field(Of String)("RFC"),
                                        Key .Subsiniestro = x.Field(Of String)("Subsiniestro"),
                                        Key .Poliza = x.Field(Of String)("Poliza")
                              }).Where(Function(s) s.Siniestro = txtSiniestro.Text.Trim() AndAlso s.Subsiniestro = cmbSubsiniestro.SelectedValue.ToString()).FirstOrDefault()
                        End If



                    Case eTipoUsuario.Proveedor
                        If String.IsNullOrWhiteSpace(txtOnBase.Text) OrElse String.IsNullOrWhiteSpace(txtSiniestro.Text) OrElse cmbSubsiniestro.Items.Count = 0 Then
                            Mensaje.MuestraMensaje("OrdenPagoSiniestros", "Folio onbase vacío", TipoMsg.Falla)
                            Return
                        End If
                        'SE AGREGA LA VALIDACION PARA LOS VARIOS CONCEPTOS
                        If chkVariosConceptos.Checked = False Then
                            oRegistro = oGrdOrden.AsEnumerable().[Select](Function(x) New With {
                                    Key .Siniestro = x.Field(Of String)("Siniestro"),
                                    Key .RFC = x.Field(Of String)("RFC"),
                                    Key .Subsiniestro = x.Field(Of String)("Subsiniestro"),
                                    Key .Poliza = x.Field(Of String)("Poliza"),
                                    Key .Factura = x.Field(Of String)("Factura")
                          }).Where(Function(s) s.Siniestro = txtSiniestro.Text.Trim() AndAlso
                                               s.RFC = txtRFC.Text.Trim() AndAlso
                                               s.Subsiniestro = cmbSubsiniestro.SelectedValue.ToString() AndAlso
                                               s.Factura = txtNumeroComprobante.Text.Trim()
                        ).FirstOrDefault()
                        End If





                End Select


                If oRegistro Is Nothing Then

                    If oGrdOrden.Rows.Count > 0 Then

                        'Se valida que todos los registros correspondan a la misma póliza
                        If Not cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then
                            oFilaSeleccion = oGrdOrden.Select(String.Format("Poliza = '{0}'", txtPoliza.Text))
                            oFilaSeleccion3 = oGrdOrden.Select(String.Format("RFC = '{0}'", txtRFC.Text))
                            oFilaSeleccion2 = oGrdOrden.Select(String.Format("Siniestro = {0}", txtSiniestro.Text.Trim))

                            If oFilaSeleccion3.Length = 0 And chkVariasFacturas.Checked = True Then 'FJCP MULTIPAGO
                                Mensaje.MuestraMensaje("OrdenPagoSiniestros", "El Beneficiario debe ser igual para un multipago", TipoMsg.Falla)
                                Return
                            End If

                        Else
                            'se comenta esta linea para realizar bien la validacion de un multipago
                            'oFilaSeleccion = oGrdOrden.Select(String.Format("Poliza = '{0}' AND Factura = '{1}'", txtPoliza.Text, txtNumeroComprobante.Text))
                            oFilaSeleccion = oGrdOrden.Select(String.Format("RFC = '{0}'", txtRFC.Text))
                            oFilaSeleccion2 = oGrdOrden.Select(String.Format("Siniestro = {0}", txtSiniestro.Text.Trim))
                        End If



                        If oFilaSeleccion2.Length = 0 And chkVariasFacturas.Checked = True Then
                            Mensaje.MuestraMensaje("OrdenPagoSiniestros", "El número de siniestro debe ser igual para un multipago", TipoMsg.Falla)
                            Return
                        End If

                        If oFilaSeleccion.Length = 0 Then
                            'FJCP MULTIPAGO INI
                            If chkVariasFacturas.Checked = False Then
                                Limpiartodo()
                            End If
                            'FJCP MULTIPAGO FIN 
                            Mensaje.MuestraMensaje("OrdenPagoSiniestros", "El RFC debe ser igual para un multipago", TipoMsg.Falla)
                            Return
                        End If

                    End If

                    'Se obtienen datos secundarios
                    Select Case cmbTipoUsuario.SelectedValue
                        Case eTipoUsuario.Asegurado, eTipoUsuario.Tercero
                            oFilaSeleccion = oSeleccionActual.Select(String.Format("nro_stro = {0} AND id_substro = {1}", txtSiniestro.Text.Trim, cmbSubsiniestro.SelectedValue))
                        Case eTipoUsuario.Proveedor
                            'oFilaSeleccion = oSeleccionActual.Select(String.Format("nro_stro = {0} AND id_substro = {1} AND folio_GMX = {2}", txtSiniestro.Text.Trim, cmbSubsiniestro.SelectedValue, txtNumeroComprobante.Text.ToString())) 'FFUENTES
                            oFilaSeleccion = oSeleccionActual.Select(String.Format("nro_stro = {0} AND id_substro = {1} ", txtSiniestro.Text.Trim, cmbSubsiniestro.SelectedValue, txtNumeroComprobante.Text.ToString())) 'FFUENTES
                        Case Else
                            oFilaSeleccion = Nothing
                    End Select

                    If Not oFilaSeleccion Is Nothing Then

                        oTabla = oGrdOrden

                        oFila = oTabla.NewRow()
                        oFila("NumeroPago") = cmbNumPago.SelectedValue.ToString() 'FJCP MEJORAS FASE II NUMERO PAGO
                        'FJCP MULTIPAGO -INI
                        oFila("FolioOnbase") = txtOnBase.Text.Trim()
                        'FJCP MULTIPAGO - FIN
                        oFila("Siniestro") = txtSiniestro.Text.Trim()
                        oFila("RFC") = txtRFC.Text.Trim()
                        oFila("Subsiniestro") = cmbSubsiniestro.SelectedValue.ToString()

                        oFila("Moneda") = txtMonedaPoliza.Text


                        'SE VA AGREGAR EL METODO PARA CARGAR LOS CONCEPTOS POR DEFAULT FFUENTES
                        ''JLC Mejoras Clase de Pago -Inicio
                        ''oFila("ClasePago") = "26"
                        oFila("ClasePago") = txt_clase.Text
                        ''JLC Mejoras Clase de Pago -Fin

                        oFila("ConceptoPago") = "350"
                        oFila("Poliza") = txtPoliza.Text.Trim()
                        oFila("TipoMoneda") = oFilaSeleccion(0).Item("Moneda_poliza")
                        oFila("Descuentos") = 0
                        oFila("Deducible") = 0

                        oFila("IdSiniestro") = oFilaSeleccion(0).Item("id_stro")
                        oFila("IdPersona") = oFilaSeleccion(0).Item("id_persona")

                        'JLC Mejoras Moneda Pago-Inicio
                        'If Me.txtMonedaPoliza.Text = "NACIONAL" Then
                        If Me.cmbMonedaPago.SelectedValue = 1 Then
                            'Me.cmbMonedaPago.SelectedValue = 0
                            'Else
                            Me.txtTipoCambio.Text = ObtenerTipoCambio().ToString
                        End If
                        'JLC Mejoras Moneda Pago-Fin

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
                                oFila("Reserva") = oFilaSeleccion(0).Item("Reserva")
                                oFila("ImportePagos") = oFilaSeleccion(0).Item("Total_Pago")
                                oFila("CodigoAsegurado") = oFilaSeleccion(0).Item("cod_aseg")
                                oFila("MonedaFactura") = 0

                            Case eTipoUsuario.Proveedor
                                oFila("Factura") = oFilaSeleccion(0).Item("folio_GMX")
                                oFila("CodigoTercero") = 0
                                oFila("CodIndCob") = 0
                                oFila("SnCondusef") = 0
                                oFila("NumeroOficioCondusef") = ""
                                oFila("FechaOficioCondusef") = ""
                                oFila("NumeroCorrelaEstim") = 0
                                oFila("Estimacion") = oFilaSeleccion(0).Item("Estimacion")
                                oFila("Reserva") = oFilaSeleccion(0).Item("Reserva")
                                'Importes e impuestos
                                'SE AGREGA LA VALIDACION PARA LOS VARIOS CONCEPTOS
                                If chkVariosConceptos.Checked = False Then
                                    oFila("Pago") = Math.Round(IIf(cmbMonedaPago.SelectedValue = 0, CDbl(oFilaSeleccion(0).Item("imp_subtotal")), CDbl(oFilaSeleccion(0).Item("imp_subtotal"))), 2)
                                Else
                                    oFila("Pago") = 0
                                End If

                                'Verificar si se queda
                                oFila("Impuestos") = CDbl(oFilaSeleccion(0).Item("imp_impuestos"))
                                oFila("Retenciones") = CDbl(oFilaSeleccion(0).Item("imp_retencion"))
                                'oFila("PagoSinIva") = CDbl(oFilaSeleccion(0).Item("PagoSinIva"))
                                'oFila("PagoConIva") = CDbl(oFilaSeleccion(0).Item("PagoConIva"))

                                oFila("FechaComprobante") = Me.txtFechaComprobante.Text.Trim
                                oFila("NumeroComprobante") = Me.txtNumeroComprobante.Text.Trim
                                oFila("CodigoAsegurado") = oFilaSeleccion(0).Item("cod_pres")
                                oFila("MonedaFactura") = oFilaSeleccion(0).Item("cod_moneda")
                        End Select

                        oFila("CodItem") = oFilaSeleccion(0).Item("cod_item")
                        oFila("CodigoRamo") = oFilaSeleccion(0).Item("Cod_ramo")
                        oFila("CodigoSubRamo") = oFilaSeleccion(0).Item("cod_subramo")
                        oFila("CodigoTipoStro") = oFilaSeleccion(0).Item("cod_tipo_stro")

                        oFila("TipoPago") = 1

                        oTabla.Columns("Pago").ReadOnly = IIf(cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor, True, False)
                        'Se agrega validacion para que no habilite el pago por que ya se carga por default proyeco interproteccion fast track
                        If (oFilaSeleccion(0).Item("Fast_track") = "SI") Then
                            oTabla.Columns("Pago").ReadOnly = True
                            oFila("FastTrack") = oFilaSeleccion(0).Item("Fast_track")
                            oTipoCuentaT_stro.Value = oFilaSeleccion(0).Item("Cuenta")
                            oCuentaBancariaT_stro.Value = oFilaSeleccion(0).Item("Cuenta_Clabe")
                            'oFila("Pago") = Math.Round(IIf(cmbMonedaPago.SelectedValue = 0, CDbl(oFilaSeleccion(0).Item("Subtotal")), CDbl(oFilaSeleccion(0).Item("Subtotal"))), 2)
                            oFila("Pago") = Math.Round(IIf(cmbMonedaPago.SelectedValue = 0, CDbl(oFilaSeleccion(0).Item("Total")), CDbl(oFilaSeleccion(0).Item("Total"))), 2) 'cambiar todos subtotal por total
                        Else
                            oFila("FastTrack") = oFilaSeleccion(0).Item("Fast_track")
                            If cmbTipoUsuario.SelectedValue <> eTipoUsuario.Proveedor Then
                                oFila("Pago") = Math.Round(IIf(cmbMonedaPago.SelectedValue = 0, CDbl(oFilaSeleccion(0).Item("Subtotal")), CDbl(oFilaSeleccion(0).Item("Subtotal"))), 2) 'VZAVALETA_1090_CC
                            Else
                                oFila("Pago") = Math.Round(IIf(cmbMonedaPago.SelectedValue = 0, CDbl(oFilaSeleccion(0).Item("imp_subtotal")), CDbl(oFilaSeleccion(0).Item("imp_subtotal"))), 2)
                            End If

                        End If

                        oTabla.Rows.Add(oFila)

                        oGrdOrden = oTabla
                        grd.DataSource = oTabla
                        grd.DataBind()


                        grd.Columns(2).Visible = IIf(cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor, True, False)
                        grd.Columns(13).Visible = IIf(cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor, False, True) 'FJCP 10290 MEJORAS Deducible

                        cmbTipoUsuario.Enabled = False

                        If cmbTipoUsuario.SelectedValue <> eTipoUsuario.Proveedor Then
                            'txtConceptoOP.Text = ""

                            'For Each oFila In oTabla.Rows

                            '    If txtConceptoOP.Text.Trim = String.Empty Then
                            '        'txtConceptoOP.Text = String.Format("{0} {1}", txtConceptoOP.Text.Trim, oFila("Siniestro"))
                            '        txtConceptoOP.Text = String.Format("{0} {1} {2}", txtConceptoOP.Text.Trim, oFila("Siniestro"), oFila("Subsiniestro")) 'FJCP MEJORAS 10290 DETALLES
                            '        substroDetalle = CInt(oFila("Subsiniestro"))
                            '    Else
                            '        'txtConceptoOP.Text = String.Format("{0}, {1}", txtConceptoOP.Text.Trim, oFila("Siniestro"))
                            '        If substroDetalle <> CInt(oFila("Subsiniestro")) Then
                            '            txtConceptoOP.Text = String.Format("{0}, {1}", txtConceptoOP.Text.Trim, oFila("Subsiniestro")) 'FJCP MEJORAS 10290 DETALLES
                            '        End If
                            '    End If
                            'Next
                            'se agrega por tema fast track
                            If (oFilaSeleccion(0).Item("Fast_track") = "SI") Then
                                CalcularTotales()
                            End If
                        End If

                        If cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then
                            CalcularTotales()
                        End If
                        'FJCP MULTIPAGO
                        txtConceptoOP.Text = ""

                        For Each oFila In oTabla.Rows

                            If txtConceptoOP.Text.Trim = String.Empty Then
                                'txtConceptoOP.Text = String.Format("{0} {1}", txtConceptoOP.Text.Trim, oFila("Siniestro"))
                                txtConceptoOP.Text = String.Format("{0} {1} {2}", txtConceptoOP.Text.Trim, oFila("Siniestro"), oFila("Subsiniestro")) 'FJCP MEJORAS 10290 DETALLES
                                substroDetalle = CInt(oFila("Subsiniestro"))
                            Else
                                'txtConceptoOP.Text = String.Format("{0}, {1}", txtConceptoOP.Text.Trim, oFila("Siniestro"))
                                If substroDetalle <> CInt(oFila("Subsiniestro")) Then
                                    txtConceptoOP.Text = String.Format("{0}, {1}", txtConceptoOP.Text.Trim, oFila("Subsiniestro")) 'FJCP MEJORAS 10290 DETALLES
                                End If
                            End If
                        Next
                        ''----------------------------------------------------
                        txtConceptoOP.Text = String.Format("{0} {1}", txtConceptoOP.Text.Trim.ToString(), oClavesPago.Select(String.Format("cod_clase_pago = '{0}'", oTabla.Rows(0)("ClasePago")))(0)("txt_desc").ToString())

                        Me.txtBeneficiario.Text = Me.txtBeneficiario_stro.Text.Trim

                        Me.txtBeneficiario_stro.Enabled = False

                        If cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then
                            'FJCP 10290 MEJORAS Pago solicitado excede a la cobertura de la póliza
                            If validaPagoVSmontoCob(oFila("Siniestro").ToString(), oFila("Subsiniestro").ToString(), oFila("Pago")) Then
                                MuestraMensaje("Alerta de Pago", "El importe de pago solicitado excede al monto de la cobertura de la póliza", TipoMsg.Advertencia)
                            End If
                        End If

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
                oFila("Subsiniestro") = cmbSubsiniestro.SelectedValue.ToString()
                oFila("Moneda") = txtMonedaPoliza.Text
                oFila("ClasePago") = "26"
                oFila("ConceptoPago") = "350"
                oFila("Poliza") = txtPoliza.Text.Trim()
                oFila("TipoMoneda") = IIf(txtMonedaPoliza.Text = "NACIONAL", 0, 1)

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

        Dim folioOnBase As String 'fjcp cuentas bancarias
        Dim ctaClabe As String
        Dim folioOnbase_edoCta As String
        Dim dt As New DataTable

        Dim bExisteCtaGob As Boolean
        Try
            bExisteCtaGob = False
            If grd.Rows.Count > 0 AndAlso cmbTipoPagoOP.SelectedValue = "T" Then


                'FJCP 10290 MEJORAS Cuentas bancarias en pagos a asegurados y terceros FAST TRACK ini
                'If Not cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then
                folioOnBase = txtOnBase.Text.Trim
                'ctaClabe = Funciones.fn_EjecutaStr("EXECUTE usp_obtenerNroCta_FolioOnBase " + folioOnBase.ToString())

                oDatos = New DataSet
                oParametros = New Dictionary(Of String, Object)
                oParametros.Add("folioOnBase", CInt(folioOnBase.ToString()))
                oDatos = Funciones.ObtenerDatos("usp_obtenerNroCta_FolioOnBase", oParametros)
                ctaClabe = oDatos.Tables(0).Rows(0).Item("Cuenta_Clabe").ToString
                folioOnbase_edoCta = oDatos.Tables(0).Rows(0).Item("Folio_Onbase_est_cuenta").ToString()
                If cmbTipoUsuario.SelectedValue <> eTipoUsuario.Proveedor Then oCuentaBancariaT_stro.Value = ctaClabe
                'End If
                'FJCP 10290 MEJORAS Cuentas bancarias en pagos a asegurados y terceros FAST TRACK fin


                oDatos = New DataSet
                oParametros = New Dictionary(Of String, Object)
                oParametros.Add("Codigo", CInt(oGrdOrden.Rows(0).Item("IdPersona")))
                oDatos = Funciones.ObtenerDatos("usp_CargarDatosBancariosBeneficiario_stro", oParametros)
                If Not oDatos Is Nothing AndAlso oDatos.Tables(0).Rows.Count > 0 Then
                    With oDatos.Tables(0).Rows(0)

                        oBancoT_stro.Value = .Item("CodigoBanco")
                        oMonedaT_stro.Value = .Item("CodigoMoneda")
                        oTipoCuentaT_stro.Value = .Item("TipoCuenta")
                        'oCuentaBancariaT_stro.Value = .Item("NumeroCuenta")
                        If cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then oCuentaBancariaT_stro.Value = .Item("NumeroCuenta")
                        If cmbTipoUsuario.SelectedValue <> eTipoUsuario.Proveedor Then oCuentaBancariaT_stro.Value = ctaClabe

                        oBeneficiarioT_stro.Value = .Item("Beneficiario")

                        oSucursalT_stro.Value = "CIUDAD DE MEXICO"
                        oBeneficiarioT_stro.Value = IIf(oBeneficiarioT_stro.Value = String.Empty, Me.txtBeneficiario.Text.Trim, oBeneficiarioT_stro.Value)
                        oBeneficiarioT_stro.Value = Me.txtBeneficiario.Text.Trim

                        oParametros.Add("Banco", oBancoT_stro.Value)
                        oParametros.Add("Sucursal", oSucursalT_stro.Value)
                        oParametros.Add("Beneficiario", oBeneficiarioT_stro.Value)
                        'oParametros.Add("Moneda", cmbMonedaPago.SelectedValue)
                        oParametros.Add("Moneda", oMonedaT_stro.Value)
                        oParametros.Add("TipoCuenta", oTipoCuentaT_stro.Value)
                        oParametros.Add("CuentaBancaria", oCuentaBancariaT_stro.Value)
                        oParametros.Add("Plaza", oPlazaT_stro.Value)
                        oParametros.Add("ABA", oAbaT_stro.Value)
                        oParametros.Add("Fasttrack", oGrdOrden.Rows(0).Item("FastTrack")) 'se agrega por proyecto de inter
                        bTieneDatosBancarios = True

                    End With

                Else

                    If Me.cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then
                        Mensaje.MuestraMensaje("Cuentas bancarias", "No existen cuentas asociadas", TipoMsg.Falla)
                        Me.cmbTipoPagoOP.SelectedValue = "C"
                        Me.btnVerCuentas.Visible = False
                        Return
                    End If

                    oSucursalT_stro.Value = "CIUDAD DE MEXICO"
                    oBeneficiarioT_stro.Value = Me.txtBeneficiario.Text.Trim
                    oBancoT_stro.Value = ""

                    'FJCP MEJORAS PAGO TESOFE INI
                    oDatos = New DataSet
                    oParametros = New Dictionary(Of String, Object)

                    oParametros.Add("Accion", 1)
                    oParametros.Add("Codigo", CInt(txtCodigoBeneficiario_stro.Text.Trim))
                    oParametros.Add("PagarA", cmbTipoUsuario.SelectedValue)

                    oDatos = Funciones.ObtenerDatos("usp_CargarDatosBancariosDepGob_stro", oParametros)

                    If Not oDatos Is Nothing AndAlso oDatos.Tables(0).Rows.Count > 0 Then

                        With oDatos.Tables(0).Rows(0)
                            oBancoT_stro.Value = .Item("CodigoBanco")
                            oMonedaT_stro.Value = cmbMonedaPago.SelectedValue
                            oTipoCuentaT_stro.Value = 2
                            oCuentaBancariaT_stro.Value = .Item("NumeroCuenta")
                        End With
                        bExisteCtaGob = True
                    End If

                    Dim ctaParam As String
                    ctaParam = Funciones.fn_EjecutaStr("usp_CargarDatosBancariosDepGob_stro @Accion = 2") 'FJCP MEJORAS PAGO TESOFE 



                    If oCuentaBancariaT_stro.Value = ctaParam Then
                        drDependencias.Visible = True
                        lblDependencias.Visible = True
                        Funciones.fn_Consulta("usp_CargarDatosBancariosDepGob_stro @Accion = 3, @Codigo = " + txtCodigoBeneficiario_stro.Text.Trim + ", @PagarA = " + cmbTipoUsuario.SelectedValue.ToString(), dt)
                        Funciones.LlenaDDL(drDependencias, dt, "codigo", "txt_desc", 0, False)
                    End If



                    'FJCP MEJORAS PAGO TESOFE FIN

                    'Se agrega validacion para que no habilite el pago por que ya se carga por default proyeco interproteccion fast track
                    If (oGrdOrden.Rows(0).Item("FastTrack") = "SI") Then
                        oParametros.Add("Banco", CInt(oCuentaBancariaT_stro.Value.Substring(0, 3)))
                        oParametros.Add("Sucursal", oSucursalT_stro.Value)
                        oParametros.Add("Beneficiario", oBeneficiarioT_stro.Value)
                        oParametros.Add("Moneda", cmbMonedaPago.SelectedValue)
                        oParametros.Add("TipoCuenta", 1)
                        oParametros.Add("CuentaBancaria", oCuentaBancariaT_stro.Value)
                        oParametros.Add("Plaza", oPlazaT_stro.Value)
                        oParametros.Add("ABA", oAbaT_stro.Value)
                        oParametros.Add("Fasttrack", oGrdOrden.Rows(0).Item("FastTrack"))
                        bTieneDatosBancarios = True  '   Con esta variable condicionamos a tomar los datos bancarios precargados de trasfernecia 
                    Else
                        oParametros.Add("Banco", oBancoT_stro.Value)
                        oParametros.Add("Sucursal", oSucursalT_stro.Value)
                        oParametros.Add("Beneficiario", oBeneficiarioT_stro.Value)
                        oParametros.Add("Moneda", cmbMonedaPago.SelectedValue)
                        oParametros.Add("TipoCuenta", oTipoCuentaT_stro.Value)
                        oParametros.Add("CuentaBancaria", oCuentaBancariaT_stro.Value)
                        oParametros.Add("Plaza", oPlazaT_stro.Value)
                        oParametros.Add("ABA", oAbaT_stro.Value)
                        oParametros.Add("Fasttrack", oGrdOrden.Rows(0).Item("FastTrack")) 'se agrega por proyecto de inter
                        bTieneDatosBancarios = False 'Con esta variable condicionamos a tomar los datos bancarios precargados de trasfernecia 
                    End If


                End If

                oParametros.Add("fOnbase_edoCta", folioOnbase_edoCta.ToString()) 'FJCP 10290 MEJORAS Habilita campo Folio OnBase Edo Cuenta
                oParametros.Add("tipoUsuario", cmbTipoUsuario.SelectedValue) 'FJCP 10290 MEJORAS Habilita campo Folio OnBase Edo Cuenta

                If bExisteCtaGob Then
                    bTieneDatosBancarios = True
                End If

                Master.MuestraTransferenciasBancariasSiniestros(IO.Path.GetFileName(Request.Url.AbsolutePath),
                                                                oCatalogoBancosT, oCatalogoTiposCuentaT, oCatalogoMonedasT,
                                                                oParametros, bTieneDatosBancarios, cmbNumPago.SelectedValue)

            End If


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
    Private Function ObtenerRFC(ByVal sCodigoUsuario As String, ByVal Accion As String, ByVal RFC As String) As Boolean

        Dim oParametros As New Dictionary(Of String, Object)

        Dim oDatos As DataSet

        Try

            ObtenerRFC = False

            oParametros = New Dictionary(Of String, Object)

            oDatos = New DataSet

            oParametros.Add("CodigoUsuario", CInt(sCodigoUsuario))
            oParametros.Add("Accion", CInt(Accion))
            oParametros.Add("RFC", RFC)

            oDatos = Funciones.ObtenerDatos("usp_ObtenerRFC_stro", oParametros)

            If Not oDatos Is Nothing AndAlso oDatos.Tables(0).Rows.Count > 0 Then
                Me.txtRFC.Text = oDatos.Tables(0).Rows(0).Item("RFC").ToString.Trim
                Me.txtCodigoBeneficiario_stro.Text = oDatos.Tables(0).Rows(0).Item("cod_tercero").ToString.Trim
                Me.txtBeneficiario_stro.Text = oDatos.Tables(0).Rows(0).Item("Nom_Tercero").ToString.Trim
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
                oSolicitudPago.AppendFormat("<TotalPagoMoneda>{0}</TotalPagoMoneda>", CDbl(iptxtTotal.Text))
                oSolicitudPago.AppendFormat("<TotalPagoNacional>{0}</TotalPagoNacional>", IIf(cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor, CDbl(iptxtTotal.Text), CDbl(iptxtTotal.Text)))
                oSolicitudPago.AppendFormat("<TotalIVA>{0}</TotalIVA>", CDbl(iptxtTotalImpuestos.Text))
                oSolicitudPago.AppendFormat("<TotalPago>{0}</TotalPago>", CDbl(iptxtTotal.Text))
                oSolicitudPago.AppendFormat("<VariasFacturas>{0}</VariasFacturas>", IIf(chkVariasFacturas.Checked, "Y", "N"))

                Select Case cmbTipoUsuario.SelectedValue
                    Case eTipoUsuario.Asegurado    'Asegurado
                        oSolicitudPago.AppendFormat("<TipoUsuario>{0}</TipoUsuario>", 7)
                        oSolicitudPago.AppendFormat("<FolioOnbase>{0}</FolioOnbase>", Me.txtOnBase.Text)
                        oSolicitudPago.AppendFormat("<TipoComprobante>{0}</TipoComprobante>", cmbTipoComprobante.SelectedValue)
                        oSolicitudPago.AppendFormat("<NumeroComprobante>{0}</NumeroComprobante>", IIf(txtNumeroComprobante.Text = String.Empty, "", txtNumeroComprobante.Text))
                        oSolicitudPago.AppendFormat("<FechaComprobante>{0}</FechaComprobante>", IIf(txtFechaComprobante.Text = String.Empty, "01/01/1900", txtFechaComprobante.Text))
                    Case eTipoUsuario.Tercero   'Tercero
                        oSolicitudPago.AppendFormat("<TipoUsuario>{0}</TipoUsuario>", 8)
                        oSolicitudPago.AppendFormat("<FolioOnbase>{0}</FolioOnbase>", Me.txtOnBase.Text)
                        oSolicitudPago.AppendFormat("<TipoComprobante>{0}</TipoComprobante>", cmbTipoComprobante.SelectedValue)
                        oSolicitudPago.AppendFormat("<NumeroComprobante>{0}</NumeroComprobante>", IIf(txtNumeroComprobante.Text = String.Empty, "", txtNumeroComprobante.Text))
                        oSolicitudPago.AppendFormat("<FechaComprobante>{0}</FechaComprobante>", IIf(txtFechaComprobante.Text = String.Empty, "01/01/1900", txtFechaComprobante.Text))
                    Case eTipoUsuario.Proveedor    'Proveedor
                        oSolicitudPago.AppendFormat("<TipoUsuario>{0}</TipoUsuario>", 10)
                        oSolicitudPago.AppendFormat("<FolioOnbase>{0}</FolioOnbase>", Me.txtOnBase.Text)
                        'oSolicitudPago.AppendFormat("<TipoComprobante>{0}</TipoComprobante>", cmbTipoComprobante.SelectedValue)
                        oSolicitudPago.AppendFormat("<TipoComprobante>{0}</TipoComprobante>", 10)
                        oSolicitudPago.AppendFormat("<NumeroComprobante>{0}</NumeroComprobante>", oGrdOrden.Rows(0).Item("NumeroComprobante")) 'FFUENTES
                        oSolicitudPago.AppendFormat("<FechaComprobante>{0}</FechaComprobante>", Convert.ToDateTime(oGrdOrden.Rows(0).Item("FechaCOmprobante")).ToString("yyyyMMdd"))
                End Select

                oSolicitudPago.AppendFormat("<IdPersona>{0}</IdPersona>", CInt(oGrdOrden.Rows(0).Item("IdPersona")))
                oSolicitudPago.AppendFormat("<CodigoAsegurado>{0}</CodigoAsegurado>", Me.txtCodigoBeneficiario_stro.Text.Trim)
                oSolicitudPago.AppendFormat("<CodigoSucursal>{0}</CodigoSucursal>", CInt(cmbSucursal.SelectedValue))
                oSolicitudPago.AppendFormat("<MonedaPago>{0}</MonedaPago>", CInt(cmbMonedaPago.SelectedValue))
                oSolicitudPago.AppendFormat("<Descripcion>{0}</Descripcion>", Me.txtConceptoOP.Text.Trim + " - " + Me.txtcpto2.Text)
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
                oSolicitudPago.AppendFormat("<CodigoOrigenPago>{0}</CodigoOrigenPago>", CInt(cmbOrigenOP.SelectedValue))
                oSolicitudPago.AppendFormat("<Observaciones>{0}</Observaciones>", Me.txtConceptoOP.Text.Trim + " - " + Me.txtcpto2.Text)
                oSolicitudPago.AppendFormat("<FechaIngreso>{0}</FechaIngreso>", Convert.ToDateTime(txtFechaContable.Text.Trim).ToString("yyyyMMdd"))
                oSolicitudPago.AppendFormat("<FechaEstimadoPago>{0}</FechaEstimadoPago>", Convert.ToDateTime(txtFechaEstimadaPago.Text.Trim).ToString("yyyyMMdd"))

                For Each oFila In oGrdOrden.Rows

                    oSolicitudPago.AppendLine("<Detalle>")
                    oSolicitudPago.AppendFormat("<fOnbase>{0}</fOnbase>", CInt(oFila.Item("FolioOnbase"))) 'FJCP MULTIPAGO 
                    oSolicitudPago.AppendFormat("<IdSiniestro>{0}</IdSiniestro>", CInt(oFila.Item("IdSiniestro")))
                    oSolicitudPago.AppendFormat("<Subsiniestro>{0}</Subsiniestro>", CInt(oFila.Item("Subsiniestro")))
                    oSolicitudPago.AppendFormat("<NumPago>{0}</NumPago>", CInt(oFila.Item("NumeroPago"))) 'FJCP MEJORAS FASE II NUMERO PAGO
                    oSolicitudPago.AppendFormat("<CodigoTercero>{0}</CodigoTercero>", CInt(oFila.Item("CodigoTercero")))
                    oSolicitudPago.AppendFormat("<ClasePago>{0}</ClasePago>", CInt(oFila.Item("ClasePago")))
                    oSolicitudPago.AppendFormat("<ConceptoPago>{0}</ConceptoPago>", CInt(oFila.Item("ConceptoPago")))

                    If cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then
                        oSolicitudPago.AppendFormat("<Pago>{0}</Pago>", CDbl(oFila.Item("Pago")) - CDbl(oFila.Item("Descuentos")))
                    Else
                        oSolicitudPago.AppendFormat("<Pago>{0}</Pago>", CDbl(oFila.Item("Pago")) - CDbl(oFila.Item("Descuentos")))
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
                    oSolicitudPago.AppendFormat("<PagoNacional>{0}</PagoNacional>", IIf(cmbMonedaPago.SelectedValue = 0, (CDbl(oFila.Item("Pago")) - CDbl(oFila.Item("Descuentos"))), (CDbl(oFila.Item("Pago")) - CDbl(oFila.Item("Descuentos"))) * CDbl(txtTipoCambio.Text)))
                    oSolicitudPago.AppendFormat("<CodigoTipoStro>{0}</CodigoTipoStro>", CInt(oFila.Item("CodigoTipoStro")))
                    oSolicitudPago.AppendFormat("<TipoPagoDetalle>{0}</TipoPagoDetalle>", CInt(oFila.Item("TipoPago")))

                    oSolicitudPago.AppendLine("</Detalle>")

                    iNumeroCorrelaPagos += 1

                Next
                'Campos para transferencia
                If cmbTipoPagoOP.SelectedValue = "T" Then
                    Select Case cmbTipoUsuario.SelectedValue
                        Case eTipoUsuario.Asegurado    'Asegurado
                            oSolicitudPago.AppendFormat("<NumeroCuentaTransferencia>{0}</NumeroCuentaTransferencia>", oCuentaBancariaT_stro.Value)
                            oSolicitudPago.AppendFormat("<CodigoBancoTransferencia>{0}</CodigoBancoTransferencia>", IIf(oBancoT_stro.Value = String.Empty, String.Empty, CInt(oBancoT_stro.Value)))
                        Case eTipoUsuario.Tercero   'Tercero
                            oSolicitudPago.AppendFormat("<NumeroCuentaTransferencia>{0}</NumeroCuentaTransferencia>", oCuentaBancariaT_stro.Value)
                            oSolicitudPago.AppendFormat("<CodigoBancoTransferencia>{0}</CodigoBancoTransferencia>", IIf(oBancoT_stro.Value = String.Empty, String.Empty, CInt(oBancoT_stro.Value)))
                        Case eTipoUsuario.Proveedor    'Proveedor
                            Dim oParametros As New Dictionary(Of String, Object)
                            Dim oDatos As DataSet
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
                            End If
                            oSolicitudPago.AppendFormat("<NumeroCuentaTransferencia>{0}</NumeroCuentaTransferencia>", oCuentaBancariaT_stro.Value)
                            oSolicitudPago.AppendFormat("<CodigoBancoTransferencia>{0}</CodigoBancoTransferencia>", IIf(oBancoT_stro.Value = String.Empty, String.Empty, CInt(oBancoT_stro.Value)))
                    End Select
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
        'esto se agrega para que tome la posicion del impuesto IVA ISR
        Dim inum_impuesto As Integer = 0

        Try

            GenerarXMLImpuestos = False

            iMonedaPoliza = CInt(oGrdOrden.Rows(0).Item("TipoMoneda"))

            oImpuestos.AppendLine("<Impuestos>")

            For Each oFila In oGrdOrden.Rows

                oDatos = New DataSet

                ObtenerDetalleImpuestos(oDatos, CInt(Me.txtCodigoBeneficiario_stro.Text), CInt(oFila("ClasePago")), CInt(oFila("ConceptoPago")), CInt(oFila("IdSiniestro")), (CDbl(oFila("Pago")) - CDbl(oFila("Descuentos"))))

                If oDatos Is Nothing AndAlso oDatos.Tables(0).Rows.Count = 0 Then
                    Throw New Exception("Error al generar detalle de impuestos.")
                End If

                ' oFila("Pago") = CDbl(oFila("Pago")) - CDbl(oFila("Descuentos"))
                'oFila("ImporteImpuesto") = oDatos.Tables(0).Rows(0).Item("ImporteImpuesto")
                'oFila("ImporteRetencion") = oDatos.Tables(0).Rows(0).Item("ImporteRetencion")


                For Each oDetalle As DataRow In oDatos.Tables(0).Rows

                    oImpuestos.AppendLine("<Detalle>")

                    oImpuestos.AppendFormat("<fOnbase>{0}</fOnbase>", CInt(oFila.Item("FolioOnbase"))) 'FJCP MULTIPAGO 
                    oImpuestos.AppendFormat("<NumPago>{0}</NumPago>", CInt(oFila.Item("NumeroPago"))) 'FJCP MEJORAS FASE II NUMERO PAGO
                    'Basado en campos de la tabla stro_op_p_impuesto_g_c
                    'El id_stro_op y cod_tercero no se mandan porque sera calculado en el sp que crea la orden de pago
                    oImpuestos.AppendFormat("<IdStro>{0}</IdStro>", CInt(oFila("IdSiniestro")))
                    oImpuestos.AppendFormat("<CodItem>{0}</CodItem>", CInt(oFila.Item("CodItem")))
                    oImpuestos.AppendFormat("<CodIndCob>{0}</CodIndCob>", CInt(oFila.Item("CodIndCob")))
                    oImpuestos.AppendFormat("<NumeroCorrelaPagos>{0}</NumeroCorrelaPagos>", iNumeroCorrelaPagos)
                    oImpuestos.AppendFormat("<CodigoConcepto>{0}</CodigoConcepto>", CInt(oDetalle("CodigoConcepto"))) 'Esto se comento por que el codigo de concepto no tenia el mismo nombre
                    'oImpuestos.AppendFormat("<CodigoConcepto>{0}</CodigoConcepto>", CInt(oDetalle("ConceptoPago")))
                    oImpuestos.AppendFormat("<CodigoImpuesto>{0}</CodigoImpuesto>", CInt(oDetalle("CodigoImpuesto")))
                    oImpuestos.AppendFormat("<CodigoGrupo>{0}</CodigoGrupo>", CInt(oDetalle("CodigoGrupo")))
                    oImpuestos.AppendFormat("<CodigoCondicion>{0}</CodigoCondicion>", CInt(oDetalle("CodigoCondicion")))

                    'Si la moneda de la póliza son dolares se calculara con el tipo de cambio
                    oImpuestos.AppendFormat("<PjeImpuesto>{0}</PjeImpuesto>", IIf(iMonedaPoliza = 0, CDbl(oDetalle("PjeImpuesto")), Math.Round(CDbl(oDetalle("PjeImpuesto")) / CDbl(Me.txtTipoCambio.Text), 2)))
                    oImpuestos.AppendFormat("<Base>{0}</Base>", IIf(iMonedaPoliza = 0, (CDbl(oFila("Pago")) - CDbl(oFila("Descuentos"))), Math.Round(CDbl(oFila("Pago") - CDbl(oFila("Descuentos"))) / CDbl(Me.txtTipoCambio.Text), 2)))
                    oImpuestos.AppendFormat("<ImporteNoGravado>{0}</ImporteNoGravado>", IIf(iMonedaPoliza = 0, CDbl(oDetalle("ImporteNoGravado")), Math.Round(CDbl(oDetalle("ImporteNoGravado")) / CDbl(Me.txtTipoCambio.Text), 2)))
                    'oImpuestos.AppendFormat("<ImporteImpuesto>{0}</ImporteImpuesto>", IIf(iMonedaPoliza = 0, CDbl(oDetalle("ImporteImpuesto")), Math.Round(CDbl(oDetalle("ImporteImpuesto")) / CDbl(Me.txtTipoCambio.Text), 2)))
                    If chkVariosConceptos.Checked = True Or chkVariasFacturas.Checked = True Then
                        If cmbMonedaPago.SelectedValue = 1 Then
                            oImpuestos.AppendFormat("<ImporteImpuesto>{0}</ImporteImpuesto>", IIf(iMonedaPoliza = 0, CDbl(oDatos.Tables(0).Rows(0).Item("ImporteImpuesto")), Math.Round(CDbl(oDatos.Tables(0).Rows(0).Item("ImporteImpuesto")), 2)))
                        Else
                            oImpuestos.AppendFormat("<ImporteImpuesto>{0}</ImporteImpuesto>", IIf(iMonedaPoliza = 0, CDbl(oDatos.Tables(0).Rows(0).Item("ImporteImpuesto")), Math.Round(CDbl(oDatos.Tables(0).Rows(0).Item("ImporteImpuesto")) / CDbl(Me.txtTipoCambio.Text), 2)))
                        End If
                    Else
                        If cmbMonedaPago.SelectedValue = 1 Then
                            oImpuestos.AppendFormat("<ImporteImpuesto>{0}</ImporteImpuesto>", IIf(iMonedaPoliza = 0, CDbl(oDatos.Tables(0).Rows(inum_impuesto).Item("ImporteImpuesto")), Math.Round(CDbl(oDatos.Tables(0).Rows(inum_impuesto).Item("ImporteImpuesto")), 2)))
                        Else
                            oImpuestos.AppendFormat("<ImporteImpuesto>{0}</ImporteImpuesto>", IIf(iMonedaPoliza = 0, CDbl(oDatos.Tables(0).Rows(inum_impuesto).Item("ImporteImpuesto")), Math.Round(CDbl(oDatos.Tables(0).Rows(inum_impuesto).Item("ImporteImpuesto")) / CDbl(Me.txtTipoCambio.Text), 2)))
                        End If
                    End If
                    oImpuestos.AppendFormat("<PjeRetencion>{0}</PjeRetencion>", IIf(iMonedaPoliza = 0, CDbl(oDetalle("PjeRetencion")), Math.Round(CDbl(oDetalle("PjeRetencion")) / CDbl(Me.txtTipoCambio.Text), 2)))
                    'oImpuestos.AppendFormat("<ImporteRetencion>{0}</ImporteRetencion>", IIf(iMonedaPoliza = 0, CDbl(oDetalle("ImporteRetencion")), Math.Round(CDbl(oDetalle("ImporteRetencion")) / CDbl(Me.txtTipoCambio.Text), 2)))

                    If chkVariosConceptos.Checked = True Or chkVariasFacturas.Checked = True Then
                        oImpuestos.AppendFormat("<ImporteRetencion>{0}</ImporteRetencion>", IIf(iMonedaPoliza = 0, CDbl(oDatos.Tables(0).Rows(0).Item("ImporteRetencion")), Math.Round(CDbl(oDatos.Tables(0).Rows(0).Item("ImporteRetencion")) / CDbl(Me.txtTipoCambio.Text), 2)))
                    Else
                        oImpuestos.AppendFormat("<ImporteRetencion>{0}</ImporteRetencion>", IIf(iMonedaPoliza = 0, CDbl(oDatos.Tables(0).Rows(inum_impuesto).Item("ImporteRetencion")), Math.Round(CDbl(oDatos.Tables(0).Rows(inum_impuesto).Item("ImporteRetencion")) / CDbl(Me.txtTipoCambio.Text), 2)))
                    End If
                    oImpuestos.AppendFormat("<CodigoTratamiento>{0}</CodigoTratamiento>", CInt(oDetalle("CodigoTratamiento")))
                    oImpuestos.AppendFormat("<Subsiniestro>{0}</Subsiniestro>", CInt(oFila.Item("Subsiniestro")))

                    oImpuestos.AppendLine("</Detalle>")

                    inum_impuesto = inum_impuesto + 1

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

            If Me.txtCodigoBeneficiario_stro.Text.Trim = String.Empty OrElse Me.txtBeneficiario_stro.Text = String.Empty Then
                Throw New Exception("Nombre o razón social no definido")
            End If

            If cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then

                If Me.txtOnBase.Text.Trim = String.Empty Then
                    Throw New Exception("Folio Onbase no definido")
                End If

            End If

            If Me.txtSiniestro.Text.Trim = String.Empty OrElse cmbSubsiniestro.Items.Count = 0 Then
                Throw New Exception("Número de siniestro no definido")
            End If

            If oGrdOrden Is Nothing OrElse oGrdOrden.Rows.Count = 0 Then
                Throw New Exception("Error en lectura de datos")
            End If

            If grd Is Nothing OrElse grd.Rows.Count = 0 Then
                Throw New Exception("Error en lectura de registros")
            End If

            If Not grd.Rows.Count = oGrdOrden.Rows.Count Then
                Throw New Exception("Número de registros desigual.")
            End If

            If cmbOrigenOP.Items.Count = 0 Then
                Throw New Exception("origen para la orden de pago no definido")
            End If

            If cmbTipoPagoOP.SelectedValue = "T" Then

                If cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor AndAlso
                    (String.IsNullOrWhiteSpace(oBancoT_stro.Value) OrElse String.IsNullOrWhiteSpace(oMonedaT_stro.Value) _
                    OrElse String.IsNullOrWhiteSpace(oTipoCuentaT_stro.Value) OrElse String.IsNullOrWhiteSpace(oCuentaBancariaT_stro.Value) _
                    OrElse String.IsNullOrWhiteSpace(oBeneficiarioT_stro.Value) OrElse String.IsNullOrWhiteSpace(oSucursalT_stro.Value) _
                    OrElse String.IsNullOrWhiteSpace(oBeneficiarioT_stro.Value)) Then

                    ObtenerDatosTransferenciaProveedor()

                End If

                'FJCP TESOFE
                If cmbTipoUsuario.SelectedValue <> eTipoUsuario.Proveedor AndAlso
                    (String.IsNullOrWhiteSpace(oBancoT_stro.Value) OrElse String.IsNullOrWhiteSpace(oMonedaT_stro.Value) _
                    OrElse String.IsNullOrWhiteSpace(oTipoCuentaT_stro.Value) OrElse String.IsNullOrWhiteSpace(oCuentaBancariaT_stro.Value) _
                    OrElse String.IsNullOrWhiteSpace(oBeneficiarioT_stro.Value) OrElse String.IsNullOrWhiteSpace(oSucursalT_stro.Value) _
                    OrElse String.IsNullOrWhiteSpace(oBeneficiarioT_stro.Value)) Then

                    If Not ObtenerDatosTransferenciaDepGob() Then
                        If drDependencias.SelectedValue = "-1" Then
                            MuestraMensaje("Dependencias", "Seleccione una dependencia", TipoMsg.Advertencia)
                            Return False
                        End If
                    End If
                Else
                    If drDependencias.Visible = True Then
                        If drDependencias.SelectedValue = "-1" Then
                            MuestraMensaje("Dependencias", "Seleccione una dependencia", TipoMsg.Advertencia)
                            Return False
                        End If
                    End If
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
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("ValidarDatos error: {0}", ex.Message), TipoMsg.Falla)
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

            Dim sDia As Int16
            Dim sMes As Int16
            Dim sAnio As Int16
            'VZAVALETA_10290_CC INI
            If txt_tipoCambioConsultado.Text <> "" Then
                ObtenerTipoCambio = txt_tipoCambioConsultado.Text
                Exit Function
            End If

            'VZAVALETA_10290_CC FIN

            sDia = DateTime.Now.Day
            sMes = DateTime.Now.Month
            sAnio = DateTime.Now.Year

            'oParametros.Add("TipoMoneda", Me.cmbMonedaPago.SelectedValue)
            oParametros.Add("TipoMoneda", 1)
            oParametros.Add("Fecha", sMes.ToString() + "/" + sDia.ToString() + "/" + sAnio.ToString())

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
    Private Function ObtenerPagarA(folioOnBAse As Integer, numPago As Integer) As Boolean 'FJCP 10290
        Dim oParametros As New Dictionary(Of String, Object)
        Dim oDatos As DataSet

        Try
            Dim onBase As Integer
            Dim checkedFac As Boolean 'chkVariasFacturas
            Dim checkedCptos As Boolean 'chkVariosConceptos
            onBase = CInt(txtOnBase.Text)
            checkedFac = chkVariasFacturas.Checked
            checkedCptos = chkVariosConceptos.Checked
            limpiarCamposTipoUsuario()
            txtOnBase.Text = onBase
            chkVariasFacturas.Checked = checkedFac
            chkVariosConceptos.Checked = checkedCptos
            ObtenerPagarA = True
            oParametros = New Dictionary(Of String, Object)
            oDatos = New DataSet
            ' If Not IsNothing(folioOnBAse) Then
            oParametros.Add("folioOnbase", folioOnBAse)
            oParametros.Add("num_pago", numPago)
            'VZAVALETA_10290_INICIO
            If cmbTipoUsuario.SelectedValue <> eTipoUsuario.Ninguno Then
                oParametros.Add("PagarA", cmbTipoUsuario.SelectedValue)
            End If
            'VZAVALETA_10290_FIN
            'End If
            'If Not IsNothing(nro_stro) Then
            '    oParametros.Add("nro_stro", nro_stro)
            'End If
            oDatos = Funciones.ObtenerDatos("usp_pagarA_folioOnbase_Stro", oParametros)
            If Not oDatos Is Nothing AndAlso oDatos.Tables(0).Rows.Count > 0 Then
                'VZAVALETA_10290_INICIO
                If Not oDatos Is Nothing AndAlso oDatos.Tables(0).Rows(0).Item("id_Pagar_A").ToString() = -1 Then
                    Mensaje.MuestraMensaje("ObtenerPagarA", "Seleccione el combo Pagar A:", TipoMsg.Falla)
                    Limpiartodo()
                    Salir = 0
                    ObtenerPagarA = False
                Else
                    ' If cmbTipoUsuario.SelectedValue <> eTipoUsuario.Ninguno Then
                    Me.cmbTipoUsuario.SelectedValue = oDatos.Tables(0).Rows(0).Item("id_Pagar_A").ToString()
                    ' End If
                    habilitarCampos()
                End If
                'VZAVALETA_10290_FIN
            Else
                ObtenerPagarA = False
                'MuestraMensaje("ObtenerPagarA", "No fue posible obtener el dato Pagar A", TipoMsg.Falla)
                'txtOnBase.Text = ""
            End If
            'cmb_SelectedIndexChanged(Me.cmbTipoUsuario.SelectedValue, Nothing)
            'cmb_SelectedIndexChanged(Me.cmbTipoUsuario, New EventArgs())
            'comboBox1_SelectedIndexChanged(comboBox1, new EventArgs()); // or (null, null)
            Page_Load(Me.cmbTipoUsuario, New EventArgs())
            Return ObtenerPagarA
        Catch ex As Exception
            Return False
            Mensaje.MuestraMensaje("ObtenerPagarA", String.Format("ObtenerPagarA error: {0}", ex.Message), TipoMsg.Falla)
        End Try
    End Function

    Private Function validaPagoVSmontoCob(nro_stro As Integer, id_substro As Integer, ByVal pago As Double) As Boolean
        Dim oParametros As New Dictionary(Of String, Object)
        Dim oDatos As DataSet
        Dim montoCobertura As Double


        validaPagoVSmontoCob = False

        Try
            validaPagoVSmontoCob = True

            oParametros = New Dictionary(Of String, Object)

            oDatos = New DataSet


            oParametros.Add("id_substro", id_substro)
            oParametros.Add("nro_stro", nro_stro)




            oDatos = Funciones.ObtenerDatos("usp_valida_monto_cob_stro", oParametros)

            If Not oDatos Is Nothing AndAlso oDatos.Tables(0).Rows.Count > 0 Then
                montoCobertura = oDatos.Tables(0).Rows(0).Item("imp_suma_aseg").ToString()
                If pago > montoCobertura Then
                    validaPagoVSmontoCob = True
                Else
                    validaPagoVSmontoCob = False
                End If
            End If


            Return validaPagoVSmontoCob
        Catch ex As Exception
            Return False
            Mensaje.MuestraMensaje("validaPagoVSmontoCob", String.Format("validaPagoVSmontoCob error: {0}", ex.Message), TipoMsg.Falla)
        End Try
    End Function
#End Region

#Region "Métodos"
    Public Sub InicializarValores()

        Try

            IniciaGrid()

            CargarBeneficiarios()

            CargarCatalogosCuentasBancarias()
            '>VZAVALETA_10290_CC7
            Me.txtOnBase.Text = String.Empty

            If Me.cmbTipoComprobante.Items.Count > 0 Then
                Me.cmbTipoComprobante.Items.Clear()
            End If
            If Me.cmbNumPago.Items.Count > 0 Then
                Me.cmbNumPago.Items.Clear()
            End If
            If Me.drBeneficiario.Items.Count > 0 Then
                Me.drBeneficiario.Items.Clear()
            End If
            '<VZAVALETA_10290_CC7

            Me.cmbTipoUsuario.Enabled = True
            Me.cmbTipoUsuario.SelectedValue = eTipoUsuario.Ninguno 'VZAVALETA_10290_INICIO

            Me.txtSiniestro.Text = String.Empty

            If Me.cmbSubsiniestro.Items.Count > 0 Then
                Me.cmbSubsiniestro.Items.Clear()
            End If

            Me.txtPoliza.Text = String.Empty
            Me.txtMonedaPoliza.Text = String.Empty
            Me.txtCodigoBeneficiario_stro.Text = String.Empty
            Me.txtBeneficiario_stro.Text = String.Empty
            Me.txtRFC.Text = String.Empty
            Me.cmbMonedaPago.SelectedValue = 0
            Me.txtTipoCambio.Text = "1.00"

            Me.grd.DataSource = Nothing
            Me.grd.DataBind()

            'Me.txtTotalAutorizacion.Text = String.Empty
            'Me.txtTotalAutorizacionNacional.Text = String.Empty
            'Me.txtTotalImpuestos.Text = String.Empty
            'Me.txtTotalRetenciones.Text = String.Empty
            'Me.txtTotal.Text = String.Empty
            'Me.txtDescuentos.Text = String.Empty

            'Me.txtTotalAutorizacionFac.Text = String.Empty
            'Me.txtTotalAutorizacionNacionalFac.Text = String.Empty
            'Me.txtTotalImpuestosFac.Text = String.Empty
            'Me.txtTotalRetencionesFac.Text = String.Empty
            'Me.txtTotalFac.Text = String.Empty

            Me.txtTotalAutorizacion.Text = String.Empty 'importe de la poliza
            Me.txtTotalImpuestos.Text = String.Empty
            Me.txtTotalRetenciones.Text = String.Empty
            Me.txtTotal.Text = String.Empty  'importe de la poliza

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
            Me.txtBeneficiario_stro.Enabled = True

            If Me.cmbOrigenOP.Items.Count > 0 Then
                Me.cmbOrigenOP.Items.Clear()
            End If

            Me.cmbTipoPagoOP.SelectedValue = "T"
            Me.txtConceptoOP.Text = String.Empty
            Me.txtcpto2.Text = String.Empty
            Me.txtSiniestro.Enabled = True

            Dim sDia As Int16
            Dim sMes As Int16
            Dim sAnio As Int16

            sDia = DateTime.Now.Day
            sMes = DateTime.Now.Month
            sAnio = DateTime.Now.Year

            'Me.txtFechaRegistro.Text = DateTime.Now.ToString("dd/MM/yyyy")

            If (sDia <= 9) Then
                sDia = "0" + sDia.ToString()
            End If

            'MMQ
            'Me.txtFechaRegistro.Text = (sDia.ToString() + "/" + sMes.ToString() + "/" + sAnio.ToString()).ToString()
            Me.txtFechaRegistro.Text = Now.ToShortDateString
            'Me.txtFechaEstimadaPago.Text = ((sDia + 2).ToString() + "/" + sMes.ToString() + "/" + sAnio.ToString()).ToString()
            ' //JLC Mejoras Fecha Estimada de pago - Inicio 
            'Me.txtFechaEstimadaPago.Text = DateAdd("d", 2, Now.ToShortDateString)
            Me.txtFechaEstimadaPago.Text = FechaEstimPago()
            ' //JLC Mejoras Fecha Estimada de pago - Fin 
            Me.txtFechaContable.Text = Me.txtFechaRegistro.Text 'FFUENTES

            Me.txtNumeroComprobante.Text = String.Empty
            Me.txtFechaComprobante.Text = String.Empty

            Me.txtFechaEstimadaPago.Style("text-align") = "center"
            Me.txtTipoCambio.Style("text-align") = "center"
            Me.txtTipoCambio.Style("width") = "80px"

            Me.btnVerCuentas.Visible = False
            'FJCP 10290 MEJORAS
            Me.linkOnBase.HRef = ""
            If Me.cmbTipoUsuario.SelectedValue = eTipoUsuario.Asegurado Then
                drBeneficiario.Visible = True
                btnNvoTercero.Visible = False
                lblNvoTercero.Visible = False
                txtBeneficiario_stro.Visible = False
            End If
            If Me.cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then
                cmbNumPago.Enabled = False
            End If
            drDependencias.Visible = False
            lblDependencias.Visible = False


        Catch ex As Exception
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("InicializarValores error: {0}", ex.Message), TipoMsg.Falla)
        End Try

    End Sub
    Public Sub ObtenerDatosTransferenciaProveedor()

        Dim oParametros As New Dictionary(Of String, Object)

        Dim oDatos As DataSet

        Try

            If grd.Rows.Count > 0 AndAlso cmbTipoPagoOP.SelectedValue = "T" Then

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
                        Me.cmbTipoPagoOP.SelectedValue = "C"
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
            If txtMonedaPoliza.Text = "NACIONAL" Then
                'JLC Mejoras Moneda Pago -Inicio
                'cmbMonedaPago.SelectedValue = 0
                'dTipoCambio = 1

                If cmbMonedaPago.SelectedValue = 0 Then

                    dTipoCambio = 1
                Else
                    dTipoCambio = ObtenerTipoCambio()

                End If
                'JLC Mejoras Moneda Pago -Fin
            Else
                dTipoCambio = ObtenerTipoCambio()
            End If

            Me.txtTipoCambio.Text = dTipoCambio

            Valida_Tipo_Pago() 'VZAVALETA_10290_CC

            For Each oFila In oGrdOrden.Rows

                dPago = IIf(IsDBNull(oFila("Pago")), 0, oFila("Pago"))
                dcod_clase_pago = IIf(IsDBNull(oFila("ClasePago")), 0, oFila("ClasePago"))
                dcod_cpto = IIf(IsDBNull(oFila("ConceptoPago")), 0, oFila("ConceptoPago"))
                dDescuentos = IIf(IsDBNull(oFila("Descuentos")), 0, oFila("Descuentos"))
                If cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then

                    dPago = dPago - dDescuentos
                    txtTotalAutorizacionFac.Text = String.Format("{0:0,0.00}", Decimal.Parse(txtTotalAutorizacionFac.Text) - dDescuentos, 2)

                    If dPago > 0 Then

                        'Si es un proveedor cuya factura haya sido registrada en pesos, se tomara la moneda de pago en pesos
                        'VZAVALETA_10290_INICIO
                        'If oFila("MonedaFactura") = 0 Then

                        '    If Not dTipoCambio = 1 Then
                        '        Mensaje.MuestraMensaje("Calculo de totales", "Factura capturada en pesos, se utilizará tipo de cambio nacional.", TipoMsg.Advertencia)
                        '        'dTipoCambio = 1
                        '        'Me.txtTipoCambio.Text = dTipoCambio
                        '        cmbMonedaPago.SelectedValue = 0
                        '    End If

                        'End If
                        'VZAVALETA_10290_FIN
                        'ObtenerImpuestos(CInt(Me.txtCodigoBeneficiario_stro.Text), CInt(oFila("ClasePago")), CInt(oFila("ConceptoPago")), CInt(oFila("IdSiniestro")), dPago, dImporteImpuesto, dImporteRetencion)
                        ObtenerImpuestos(CInt(Me.txtCodigoBeneficiario_stro.Text), dcod_clase_pago, dcod_cpto, CInt(oFila("IdSiniestro")), dPago, dImporteImpuesto, dImporteRetencion)

                        If dImporteImpuesto = -1 AndAlso dImporteRetencion = -1 Then
                            'se agrego este filtro para varios conceptos
                            If chkVariosConceptos.Checked = False Then
                                Mensaje.MuestraMensaje("Calculo de totales", "No se encontro información para el cálculo de impuestos", TipoMsg.Falla)
                                If txtTotalAutorizacionNacionalFac.Text = "" Then
                                    txtTotalAutorizacionNacionalFac.Text = 0
                                End If
                                If txtTotalAutorizacionFac.Text = "" Then
                                    txtTotalAutorizacionFac.Text = 0
                                End If
                                If txtTotalImpuestosFac.Text = "" Then
                                    txtTotalImpuestosFac.Text = 0
                                End If
                                If txtTotalRetencionesFac.Text = "" Then
                                    txtTotalRetencionesFac.Text = 0
                                End If
                                If txtTotalFac.Text = "" Then
                                    txtTotalFac.Text = 0
                                End If
                                If txtTotalNacionalFac.Text = "" Then
                                    txtTotalNacionalFac.Text = 0
                                End If

                                txtTotalAutorizacionNacionalFac.Text = String.Format("{0:0,0.00}", Math.Round(Double.Parse(txtTotalAutorizacionNacionalFac.Text), 2))
                                txtTotalAutorizacionFac.Text = String.Format("{0:0,0.00}", Math.Round(Double.Parse(txtTotalAutorizacionFac.Text), 2))
                                txtTotalImpuestosFac.Text = String.Format("{0:0,0.00}", Math.Round(Double.Parse(txtTotalImpuestosFac.Text), 2))
                                txtTotalRetencionesFac.Text = String.Format("{0:0,0.00}", Math.Round(Double.Parse(txtTotalRetencionesFac.Text), 2))
                                txtTotalFac.Text = String.Format("{0:0,0.00}", Math.Round(Double.Parse(txtTotalFac.Text), 2))
                                txtTotalNacionalFac.Text = String.Format("{0:0,0.00}", Math.Round(Double.Parse(txtTotalNacionalFac.Text), 2))

                                    If dcod_clase_pago = 26 AndAlso dImporteImpuesto = -1 AndAlso dImporteRetencion = -1 Then
                                        txtTotalAutorizacion.Text = dPago
                                        txtTotalImpuestos.Text = 0
                                        txtTotalRetenciones.Text = 0
                                        txtTotal.Text = dPago
                                        txtTotalNacional.Text = dPago

                                        txtTotalAutorizacionNacionalFac.Text = String.Format("{0:0,0.00}", Math.Round(Double.Parse(txtTotalAutorizacionNacionalFac.Text), 2))
                                        txtTotalAutorizacionFac.Text = String.Format("{0:0,0.00}", Math.Round(Double.Parse(txtTotalAutorizacionFac.Text), 2))
                                        txtTotalImpuestosFac.Text = String.Format("{0:0,0.00}", Math.Round(Double.Parse(txtTotalImpuestosFac.Text), 2))
                                        txtTotalRetencionesFac.Text = String.Format("{0:0,0.00}", Math.Round(Double.Parse(txtTotalRetencionesFac.Text), 2))
                                        txtTotalFac.Text = String.Format("{0:0,0.00}", Math.Round(Double.Parse(txtTotalFac.Text), 2))
                                        txtTotalNacionalFac.Text = String.Format("{0:0,0.00}", Math.Round(Double.Parse(txtTotalNacionalFac.Text), 2))

                                    End If
                                    dImporteImpuesto = 0
                                    dImporteRetencion = 0

                                Else
                                    'varios conceptos
                                    txtTotalAutorizacion.Text = dPago + txtTotalAutorizacion.Text
                                txtTotalImpuestos.Text = dImporteImpuesto + txtTotalImpuestos.Text
                                txtTotalRetenciones.Text = dImporteRetencion + txtTotalRetenciones.Text
                                txtTotal.Text = dPago + txtTotal.Text
                                txtTotalNacional.Text = dPago + txtTotalNacional.Text

                                'varios conceptos
                                iptxtTotalAutorizacion.Text = dPago + iptxtTotalAutorizacion.Text
                                iptxtTotalImpuestos.Text = dImporteImpuesto + iptxtTotalImpuestos.Text
                                iptxtTotalRetenciones.Text = dImporteRetencion + iptxtTotalRetenciones.Text
                                iptxtTotal.Text = dPago + iptxtTotal.Text
                                iptxtTotalNacional.Text = dPago + iptxtTotalNacional.Text

                            End If

                        ElseIf (dImporteImpuesto = 0 AndAlso dImporteRetencion = 0) OrElse
                            (dImporteImpuesto = -1 OrElse dImporteRetencion = -1) Then
                            If chkVariosConceptos.Checked = True Then
                                If dImporteImpuesto = 0 AndAlso dImporteRetencion = 0 Then 'esto es cuando calcula los impuesto al cero

                                    'varios conceptos
                                    txtTotalAutorizacion.Text = dPago + txtTotalAutorizacion.Text
                                    txtTotalImpuestos.Text = dImporteImpuesto + txtTotalImpuestos.Text
                                    txtTotalRetenciones.Text = dImporteRetencion + txtTotalRetenciones.Text
                                    txtTotal.Text = dPago + txtTotal.Text
                                    txtTotalNacional.Text = dPago + txtTotalNacional.Text

                                    'varios conceptos
                                    iptxtTotalAutorizacion.Text = dPago + iptxtTotalAutorizacion.Text
                                    iptxtTotalImpuestos.Text = dImporteImpuesto + iptxtTotalImpuestos.Text
                                    iptxtTotalRetenciones.Text = dImporteRetencion + iptxtTotalRetenciones.Text
                                    iptxtTotal.Text = dPago + iptxtTotal.Text
                                    iptxtTotalNacional.Text = dPago + iptxtTotalNacional.Text

                                Else
                                    Mensaje.MuestraMensaje("Calculo de totales", "Cálculo incompleto de impuestos " + dImporteImpuesto.ToString() + dImporteRetencion.ToString(), TipoMsg.Falla)
                                End If
                            Else
                                Mensaje.MuestraMensaje("Calculo de totales", "Cálculo incompleto de impuestos " + dImporteImpuesto.ToString() + dImporteRetencion.ToString(), TipoMsg.Falla)
                            End If





                            Return
                        End If

                        'FJCP Mejoras Multipago -ini
                        If chkVariasFacturas.Checked = True Then


                            'varios conceptos
                            txtTotalAutorizacion.Text = dPago + txtTotalAutorizacion.Text
                            txtTotalImpuestos.Text = dImporteImpuesto + txtTotalImpuestos.Text
                            txtTotalRetenciones.Text = dImporteRetencion + txtTotalRetenciones.Text
                            'txtTotal.Text = txtTotal.Text + dPago + dImporteImpuesto + dImporteRetencion
                            txtTotal.Text = txtTotal.Text + dPago + dImporteImpuesto - dImporteRetencion 'VZAVALETA_10290_CC8
                            txtTotalNacional.Text = dPago + txtTotalNacional.Text

                            'varios conceptos
                            iptxtTotalAutorizacion.Text = dPago + iptxtTotalAutorizacion.Text
                            iptxtTotalImpuestos.Text = dImporteImpuesto + iptxtTotalImpuestos.Text
                            iptxtTotalRetenciones.Text = dImporteRetencion + iptxtTotalRetenciones.Text
                            iptxtTotal.Text = dPago + iptxtTotal.Text
                            iptxtTotalNacional.Text = dPago + iptxtTotalNacional.Text


                            txtTotalAutorizacionFac.Text = String.Format("{0:0,0.00}", Math.Round(Double.Parse(txtTotalAutorizacion.Text), 2))
                            txtTotalImpuestosFac.Text = String.Format("{0:0,0.00}", Math.Round(Double.Parse(txtTotalImpuestos.Text), 2))
                            txtTotalRetencionesFac.Text = String.Format("{0:0,0.00}", Math.Round(Double.Parse(txtTotalRetenciones.Text), 2))
                            txtTotalFac.Text = String.Format("{0:0,0.00}", Math.Round(Double.Parse(txtTotal.Text), 2))
                            txtTotalNacionalFac.Text = String.Format("{0:0,0.00}", Math.Round(Double.Parse(txtTotalNacional.Text), 2))
                            txtTotalAutorizacionNacionalFac.Text = String.Format("{0:0,0.00}", Math.Round(Double.Parse(txtTotalAutorizacion.Text), 2))

                        End If
                        'FJCP Mejoras Multipago -fin

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
                                'se agrego este filtro para varios conceptos
                                If chkVariosConceptos.Checked = False Then
                                    dTotalAutorizacion += dPago
                                    dTotalImpuestos += IIf(cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor, dImporteImpuesto, 0)
                                    dTotalRetenciones += IIf(cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor, dImporteRetencion, 0)

                                    dTotalAutorizacionNacional += dPago
                                    dTotalImpuestosNacional = 0
                                Else
                                    dTotalAutorizacion += dPago
                                    dTotalImpuestos += IIf(cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor, dImporteImpuesto, 0)
                                    If dImporteImpuesto = -1 Then 'Se agrego por que estaba restando un menos uno
                                        dTotalImpuestos = dTotalImpuestos + 1
                                    End If
                                    dTotalRetenciones += IIf(cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor, dImporteRetencion, 0)
                                    If dImporteRetencion = -1 Then  'Se agrego por que estaba restando un menos uno
                                        dTotalRetenciones = dTotalRetenciones + 1
                                    End If

                                    If dTotalImpuestos = -1 Then
                                        dTotalImpuestos = 0
                                    End If
                                    If dTotalRetenciones = -1 Then
                                        dTotalRetenciones = 0
                                    End If
                                    dTotalAutorizacionNacional += dPago
                                    dTotalImpuestosNacional = 0
                                End If
                            Case 1
                                'dTotalAutorizacion += (dPago / dTipoCambio)
                                'dTotalImpuestos += IIf(cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor, (dImporteImpuesto / dTipoCambio), 0)
                                'dTotalRetenciones += IIf(cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor, (dImporteRetencion / dTipoCambio), 0)

                                'dTotalAutorizacionNacional += dPago
                                'dTotalImpuestosNacional += (dImporteImpuesto - dImporteRetencion)

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

                                'JLC Mejoras Moneda Pago -Inicio
                                dTotalAutorizacion += dPago
                                dTotalImpuestos += IIf(cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor, (dImporteImpuesto), 0)
                                dTotalRetenciones += IIf(cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor, (dImporteRetencion), 0)

                                dTotalAutorizacionNacional += dPago
                                dTotalImpuestosNacional += (dImporteImpuesto - dImporteRetencion)
                                'JLC Mejoras Moneda Pago -Fin

                        End Select





                End Select

            Next

            Select Case cmbTipoUsuario.SelectedValue
                Case eTipoUsuario.Asegurado, eTipoUsuario.Tercero


                    If txtMonedaPoliza.Text = "DOLAR AMERICANO" Then
                        If cmbMonedaPago.SelectedValue = 0 Then
                            Me.txtTotalAutorizacion.Text = String.Format("{0:0,0.00}", Math.Round(dTotalAutorizacion / txtTipoCambio.Text, 2))
                            Me.txtTotalImpuestos.Text = String.Format("{0:0,0.00}", Math.Round(dTotalImpuestos / txtTipoCambio.Text, 2))
                            Me.txtTotalRetenciones.Text = String.Format("{0:0,0.00}", Math.Round(dTotalRetenciones / txtTipoCambio.Text, 2))
                            Me.txtTotal.Text = String.Format("{0:0,0.00}", Math.Round((dTotalAutorizacion / txtTipoCambio.Text) + dTotalImpuestos - dTotalRetenciones, 2))
                            Me.txtTotalNacional.Text = String.Format("{0:0,0.00}", dTotalAutorizacionNacional / txtTipoCambio.Text)
                        Else
                            Me.txtTotalAutorizacion.Text = String.Format("{0:0,0.00}", Math.Round(dTotalAutorizacion, 2))
                            Me.txtTotalImpuestos.Text = String.Format("{0:0,0.00}", Math.Round(dTotalImpuestos, 2))
                            Me.txtTotalRetenciones.Text = String.Format("{0:0,0.00}", Math.Round(dTotalRetenciones, 2))
                            Me.txtTotal.Text = String.Format("{0:0,0.00}", Math.Round(dTotalAutorizacion + dTotalImpuestos - dTotalRetenciones, 2))
                            Me.txtTotalNacional.Text = String.Format("{0:0,0.00}", dTotalAutorizacionNacional)
                        End If
                    Else
                        'JLC Mejoras Moneda Pago-Inicio
                        If cmbMonedaPago.SelectedValue = 0 Then
                            Me.txtTotalAutorizacion.Text = String.Format("{0:0,0.00}", Math.Round(dTotalAutorizacion, 2))
                            Me.txtTotalImpuestos.Text = String.Format("{0:0,0.00}", Math.Round(dTotalImpuestos, 2))
                            Me.txtTotalRetenciones.Text = String.Format("{0:0,0.00}", Math.Round(dTotalRetenciones, 2))
                            Me.txtTotal.Text = String.Format("{0:0,0.00}", Math.Round(dTotalAutorizacion + dTotalImpuestos - dTotalRetenciones, 2))
                            Me.txtTotalNacional.Text = String.Format("{0:0,0.00}", dTotalAutorizacionNacional)
                        Else

                            Me.txtTotalAutorizacion.Text = String.Format("{0:0,0.00}", Math.Round(dTotalAutorizacion * txtTipoCambio.Text, 2))
                            Me.txtTotalImpuestos.Text = String.Format("{0:0,0.00}", Math.Round(dTotalImpuestos * txtTipoCambio.Text, 2))
                            Me.txtTotalRetenciones.Text = String.Format("{0:0,0.00}", Math.Round(dTotalRetenciones * txtTipoCambio.Text, 2))
                            Me.txtTotal.Text = String.Format("{0:0,0.00}", Math.Round((dTotalAutorizacion * txtTipoCambio.Text) + dTotalImpuestos - dTotalRetenciones, 2))
                            Me.txtTotalNacional.Text = String.Format("{0:0,0.00}", dTotalAutorizacionNacional * txtTipoCambio.Text)

                        End If
                        'JLC Mejoras Moneda Pago-Fin
                    End If

                    'Cambiara segun si la moneda de pago son pesos o dolares

                    Me.iptxtTotalAutorizacion.Text = String.Format("{0:0,0.00}", Math.Round(dTotalAutorizacion, 2))
                    Me.iptxtTotalImpuestos.Text = String.Format("{0:0,0.00}", Math.Round(dTotalImpuestos, 2))
                    Me.iptxtTotalRetenciones.Text = String.Format("{0:0,0.00}", Math.Round(dTotalRetenciones, 2))
                    Me.iptxtTotal.Text = String.Format("{0:0,0.00}", Math.Round(dTotalAutorizacion + dTotalImpuestos - dTotalRetenciones, 2))
                    Me.iptxtTotalNacional.Text = String.Format("{0:0,0.00}", dTotalAutorizacionNacional)
                    'Si la moneda de la póliza es nacional se obtendran total de pago en pesos, si no los tomará del total de autorización
                    'que seran dolares
                    'Me.iptxtTotalAutorizacion.Text = String.Format("{0:0,0.00}", Math.Round(IIf(Me.txtMonedaPoliza.Text = "NACIONAL", dTotalAutorizacion, dTotalAutorizacionNacional), 2))

                    'Los impuestos y retenciones deben ser cero para asegurados y terceros
                    'El valor del total sera el total en la moneda la cual este registrada la póliza

                Case eTipoUsuario.Proveedor
                    If txtMonedaPoliza.Text = "DOLAR AMERICANO" Then
                        If cmbMonedaPago.SelectedValue = 0 Then
                            Me.txtTotalAutorizacion.Text = String.Format("{0:0,0.00}", Math.Round(dTotalAutorizacion / txtTipoCambio.Text, 2))
                            Me.txtTotalImpuestos.Text = String.Format("{0:0,0.00}", Math.Round(dTotalImpuestos / txtTipoCambio.Text, 2))
                            Me.txtTotalRetenciones.Text = String.Format("{0:0,0.00}", Math.Round(dTotalRetenciones / txtTipoCambio.Text, 2))
                            Me.txtTotal.Text = String.Format("{0:0,0.00}", Math.Round((dTotalAutorizacion / txtTipoCambio.Text) + (dTotalImpuestos / txtTipoCambio.Text) - (dTotalRetenciones / txtTipoCambio.Text), 2))
                            Me.txtTotalNacional.Text = String.Format("{0:0,0.00}", dTotalAutorizacionNacional / txtTipoCambio.Text)
                        Else
                            Me.txtTotalAutorizacion.Text = String.Format("{0:0,0.00}", Math.Round(dTotalAutorizacion, 2))
                            Me.txtTotalImpuestos.Text = String.Format("{0:0,0.00}", Math.Round(dTotalImpuestos, 2))
                            Me.txtTotalRetenciones.Text = String.Format("{0:0,0.00}", Math.Round(dTotalRetenciones, 2))
                            Me.txtTotal.Text = String.Format("{0:0,0.00}", Math.Round(dTotalAutorizacion + dTotalImpuestos - dTotalRetenciones, 2))
                            Me.txtTotalNacional.Text = String.Format("{0:0,0.00}", dTotalAutorizacionNacional)
                        End If
                    Else
                        'JLC Mejoras Moneda Pago-Inicio
                        If cmbMonedaPago.SelectedValue = 0 Then
                            Me.txtTotalAutorizacion.Text = String.Format("{0:0,0.00}", Math.Round(dTotalAutorizacion, 2))
                            Me.txtTotalImpuestos.Text = String.Format("{0:0,0.00}", Math.Round(dTotalImpuestos, 2))
                            Me.txtTotalRetenciones.Text = String.Format("{0:0,0.00}", Math.Round(dTotalRetenciones, 2))
                            Me.txtTotal.Text = String.Format("{0:0,0.00}", Math.Round(dTotalAutorizacion + dTotalImpuestos - dTotalRetenciones, 2))
                            Me.txtTotalNacional.Text = String.Format("{0:0,0.00}", dTotalAutorizacionNacional)
                        Else

                            Me.txtTotalAutorizacion.Text = String.Format("{0:0,0.00}", Math.Round(dTotalAutorizacion * txtTipoCambio.Text, 2))
                            Me.txtTotalImpuestos.Text = String.Format("{0:0,0.00}", Math.Round(dTotalImpuestos * txtTipoCambio.Text, 2))
                            Me.txtTotalRetenciones.Text = String.Format("{0:0,0.00}", Math.Round(dTotalRetenciones * txtTipoCambio.Text, 2))
                            Me.txtTotal.Text = String.Format("{0:0,0.00}", Math.Round((dTotalAutorizacion * txtTipoCambio.Text) + dTotalImpuestos - dTotalRetenciones, 2))
                            Me.txtTotalNacional.Text = String.Format("{0:0,0.00}", dTotalAutorizacionNacional * txtTipoCambio.Text)

                        End If
                        'JLC Mejoras Moneda Pago-Fin

                    End If

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
                'VZAVALETA_GMX-10290_INCIDENCIAS
                If CDbl(oDatos.Tables(0).Rows(0).Item("TotalImpuestos")) = 0 And CDbl(oDatos.Tables(0).Rows(0).Item("TotalRetenciones")) = 0 Then
                    dImpuesto = -1 'lo cambio por que esta descontando un menos uno 
                    dRetencion = -1 'lo cambio por que esta descontando un menos uno
                Else
                    dImpuesto = CDbl(oDatos.Tables(0).Rows(0).Item("TotalImpuestos"))
                    dRetencion = CDbl(oDatos.Tables(0).Rows(0).Item("TotalRetenciones"))
                End If
                'VZAVALETA_GMX-10290_INCIDENCIAS
            Else
                dImpuesto = -1 'lo cambio por que esta descontando un menos uno 
                dRetencion = -1 'lo cambio por que esta descontando un menos uno
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

    Public Sub limpiarCamposImporte()
        Me.txtTotalAutorizacion.Text = "" 'importe de la poliza
        Me.txtTotalImpuestos.Text = ""
        Me.txtTotalRetenciones.Text = ""
        Me.txtTotal.Text = ""  'importe de la poliza

        Me.iptxtTotalAutorizacion.Text = "" 'importe de pago
        Me.iptxtTotalImpuestos.Text = ""
        Me.iptxtTotal.Text = ""
        Me.iptxtTotalRetenciones.Text = ""
        Me.iptxtTotalNacional.Text = ""

        Me.txtTotalAutorizacionFac.Text = "" 'txt de facturas
        Me.txtTotalImpuestosFac.Text = ""
        Me.txtTotalRetencionesFac.Text = ""
        Me.txtTotalFac.Text = ""
        Me.txtTotalAutorizacionNacionalFac.Text = ""
        Me.txtDescuentos.Text = "" 'txt de facturas
    End Sub

    Public Sub Limpiartodo()
        Dim chkdelete As CheckBox
        For Each row In grd.Rows
            chkdelete = BuscarControlPorID(row, "eliminar")
            chkdelete.Checked = True
        Next
        EliminarFila(1)

        txtOnBase.Text = ""
        txtSiniestro.Text = ""
        cmbSubsiniestro.Items.Clear()
        txtPoliza.Text = ""
        txtMonedaPoliza.Text = ""
        txtCodigoBeneficiario_stro.Text = ""
        txtBeneficiario_stro.Text = ""
        txtRFC.Text = ""
        txtTipoCambio.Text = ""

        'cmbTipoComprobante.Items.Clear()
        txtNumeroComprobante.Text = ""
        txtFechaComprobante.Text = ""

        'txtTotalAutorizacionNacional.Text = ""
        'txtTotalAutorizacion.Text = ""
        'txtTotalImpuestos.Text = ""
        'txtTotalRetenciones.Text = ""
        'txtTotal.Text = ""
        'txtTotalNacional.Text = ""
        'txtDescuentos.Text = ""
        'Limpia las cajas de los impuestos de la factura
        'txtTotalAutorizacionNacionalFac.Text = ""
        'txtTotalAutorizacionFac.Text = ""
        'txtTotalImpuestosFac.Text = ""
        'txtTotalRetencionesFac.Text = ""
        'txtTotalFac.Text = ""
        'txtTotalNacionalFac.Text = ""

        Me.txtTotalAutorizacion.Text = "" 'importe de la poliza
        Me.txtTotalImpuestos.Text = ""
        Me.txtTotalRetenciones.Text = ""
        Me.txtTotal.Text = ""  'importe de la poliza

        Me.iptxtTotalAutorizacion.Text = "" 'importe de pago
        Me.iptxtTotalImpuestos.Text = ""
        Me.iptxtTotal.Text = ""
        'Me.iptxtTotalAutorizacionNacional.Text = ""  'importe de pago

        Me.txtTotalAutorizacionFac.Text = "" 'txt de facturas
        Me.txtTotalImpuestosFac.Text = ""
        Me.txtTotalRetencionesFac.Text = ""
        Me.txtTotalFac.Text = ""
        Me.txtTotalAutorizacionNacionalFac.Text = ""
        Me.txtDescuentos.Text = "" 'txt de facturas

        Me.lbldescuento.Text = ""

        txtBeneficiario.Text = ""
        txtConceptoOP.Text = ""
        txtcpto2.Text = ""
        cmbOrigenOP.Items.Clear()
        cmbTipoComprobante.Items.Clear()
        txtBeneficiario.Text = ""



        cmbTipoUsuario.Enabled = True
        chkVariasFacturas.Checked = False
        chkVariosConceptos.Checked = False

        'FJCP 10290 MEJORAS Nombre o Razón Social - Asegurado
        Dim dt As New DataTable
        drBeneficiario.DataSource = dt
        drBeneficiario.DataBind()


        Dim dt1 As New DataTable
        cmbNumPago.DataSource = dt1
        cmbNumPago.DataBind()
    End Sub
    Public Sub LimpiarOrdenPago() Handles btnLimpiar.Click
        Limpiartodo()
        'Dim chkdelete As CheckBox
        'For Each row In grd.Rows
        '    chkdelete = BuscarControlPorID(row, "eliminar")
        '    chkdelete.Checked = True
        'Next
        'EliminarFila(1)

        'txtOnBase.Text = ""
        'txtSiniestro.Text = ""
        'cmbSubsiniestro.Items.Clear()
        'txtPoliza.Text = ""
        'txtMonedaPoliza.Text = ""
        'txtCodigoBeneficiario_stro.Text = ""
        'txtBeneficiario_stro.Text = ""
        'txtRFC.Text = ""
        'txtTipoCambio.Text = ""

        ''cmbTipoComprobante.Items.Clear()
        'txtNumeroComprobante.Text = ""
        'txtFechaComprobante.Text = ""

        'txtTotalAutorizacionNacional.Text = ""
        'txtTotalAutorizacion.Text = ""
        'txtTotalImpuestos.Text = ""
        'txtTotalRetenciones.Text = ""
        'txtTotal.Text = ""
        'txtTotalNacional.Text = ""

        'txtBeneficiario.Text = ""
        'txtConceptoOP.Text = ""
        'cmbOrigenOP.Items.Clear()
        'txtBeneficiario.Text = ""

        'cmbTipoUsuario.Enabled = True

    End Sub
    Public Function ValidarImpuestosOPFac() As Boolean

        Dim iTotalAutorizacion As Decimal
        Dim iTotalImpuestosn As Decimal
        Dim iTotalRetenciones As Decimal
        Dim iSubTotal As Decimal
        Dim Reserva_total As Decimal
        Dim num_filas As Int16
        'optenemos el numero de filas del grid y sumamos el total de la reserva
        num_filas = oGrdOrden.Rows.Count
        num_filas = num_filas - 1
        For i = 0 To num_filas
            Reserva_total = Reserva_total + CDbl(oGrdOrden.Rows(i)("Reserva"))
        Next i

        Try
            If cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then
                'Validar los impuestos y totales de la factura con el calculo del sii
                If txtMonedaPoliza.Text = "NACIONAL" Then
                    iTotalAutorizacion = (Decimal.Parse(iptxtTotalAutorizacion.Text) + txtDescuentos.Text) - Decimal.Parse(txtTotalAutorizacionNacionalFac.Text)
                    iTotalImpuestosn = Decimal.Parse(iptxtTotalImpuestos.Text) - Decimal.Parse(txtTotalImpuestosFac.Text)
                    iTotalRetenciones = Decimal.Parse(iptxtTotalRetenciones.Text) - Decimal.Parse(txtTotalRetencionesFac.Text)
                    iSubTotal = Decimal.Parse(iptxtTotal.Text) - Decimal.Parse(txtTotalFac.Text)
                Else
                    iTotalAutorizacion = (Decimal.Parse(iptxtTotalAutorizacion.Text)) + (Decimal.Parse(txtDescuentos.Text)) - Decimal.Parse(txtTotalAutorizacionNacionalFac.Text)
                    iTotalImpuestosn = (Decimal.Parse(txtTotalImpuestosFac.Text)) - Decimal.Parse(iptxtTotalImpuestos.Text)
                    iTotalRetenciones = (Decimal.Parse(txtTotalRetencionesFac.Text)) - Decimal.Parse(iptxtTotalRetenciones.Text)
                    iSubTotal = (Decimal.Parse(txtTotalFac.Text)) - Decimal.Parse(iptxtTotal.Text)
                End If

                '--Autorizacion
                If Math.Abs(iTotalAutorizacion) > 0.9 Then
                    ValidarImpuestosOPFac = False
                    Mensaje.MuestraMensaje("OrdenPagoSiniestros", "Diferencia de Autorizacion: " + iTotalAutorizacion.ToString(), TipoMsg.Falla)
                Else
                    '--impuestos
                    If Math.Abs(iTotalImpuestosn) > 0.9 Then
                        ValidarImpuestosOPFac = False
                        Mensaje.MuestraMensaje("OrdenPagoSiniestros", "Diferencia de iTotalImpuestosn: " + iTotalImpuestosn.ToString(), TipoMsg.Falla)
                    Else
                        '--retenciones
                        If Math.Abs(iTotalRetenciones) > 0.9 Then
                            ValidarImpuestosOPFac = False
                            Mensaje.MuestraMensaje("OrdenPagoSiniestros", "Diferencia de iTotalRetenciones: " + iTotalRetenciones.ToString(), TipoMsg.Falla)
                        Else
                            '--subtotal
                            If Math.Abs(iSubTotal) > 0.9 Then
                                ValidarImpuestosOPFac = False
                                Mensaje.MuestraMensaje("OrdenPagoSiniestros", "Diferencia de iSubTotal: " + iSubTotal.ToString(), TipoMsg.Falla)
                            Else
                                If oGrdOrden.Rows(0)("ClasePago") = 26 Or oGrdOrden.Rows(0)("ClasePago") = 27 Or oGrdOrden.Rows(0)("ClasePago") = 28 Then
                                    'Validamos el pago deacuerdo a la reserva 
                                    If txtMonedaPoliza.Text = "DOLAR AMERICANO" And cmbMonedaPago.SelectedValue = 0 Then
                                        If CDbl(txtTotalAutorizacion.Text) > Reserva_total Then
                                            ValidarImpuestosOPFac = False
                                            Mensaje.MuestraMensaje("OrdenPagoSiniestros", "El pago: " + txtTotalAutorizacion.Text.ToString() + " Mayor a la Reserva: " + Reserva_total.ToString(), TipoMsg.Falla)
                                        Else
                                            'Debe estar en true, esto significa que no ubo diferencias en los impuestos
                                            ValidarImpuestosOPFac = True
                                        End If
                                    Else
                                        If CDbl(iptxtTotalAutorizacion.Text) > Reserva_total Then
                                            ValidarImpuestosOPFac = False
                                            Mensaje.MuestraMensaje("OrdenPagoSiniestros", "El pago: " + iptxtTotalAutorizacion.Text.ToString() + " Mayor a la Reserva: " + Reserva_total.ToString(), TipoMsg.Falla)
                                        Else
                                            'Debe estar en true, esto significa que no ubo diferencias en los impuestos
                                            ValidarImpuestosOPFac = True
                                        End If
                                    End If
                                Else
                                    'Debe estar en true, esto significa que no ubo diferencias en los impuestos
                                    ValidarImpuestosOPFac = True
                                End If
                            End If
                        End If
                    End If
                End If
            Else
                'ASEGURADOS Y TERCEROS
                'para el caso de asegurado y terceros que no tienen descuentos
                'Validamos el pago deacuerdo a la reserva 
                If oGrdOrden.Rows(0)("ClasePago") = 26 Or oGrdOrden.Rows(0)("ClasePago") = 27 Or oGrdOrden.Rows(0)("ClasePago") = 28 Then
                    'Validamos el pago deacuerdo a la reserva 
                    If txtMonedaPoliza.Text = "DOLAR AMERICANO" And cmbMonedaPago.SelectedValue = 0 Then
                        If CDbl(txtTotalAutorizacion.Text) > Reserva_total Then
                            ValidarImpuestosOPFac = False
                            Mensaje.MuestraMensaje("OrdenPagoSiniestros", "El pago: " + txtTotalAutorizacion.Text.ToString() + " Mayor a la Reserva: " + Reserva_total.ToString(), TipoMsg.Falla)
                        Else
                            'Debe estar en true, esto significa que no ubo diferencias en los impuestos
                            ValidarImpuestosOPFac = True
                        End If
                    Else
                        If CDbl(iptxtTotalAutorizacion.Text) > Reserva_total Then
                            ValidarImpuestosOPFac = False
                            Mensaje.MuestraMensaje("OrdenPagoSiniestros", "El pago: " + iptxtTotalAutorizacion.Text.ToString() + " Mayor a la Reserva: " + Reserva_total.ToString(), TipoMsg.Falla)
                        Else
                            'Debe estar en true, esto significa que no ubo diferencias en los impuestos
                            ValidarImpuestosOPFac = True
                        End If
                    End If
                Else
                    ValidarImpuestosOPFac = True
                End If
            End If
        Catch ex As Exception
            ValidarImpuestosOPFac = False
        End Try
    End Function
    Public Sub GenerarOrdenPago() Handles btnGrabarOP.Click

        Dim oSolicitudPago, oImpuestos As StringBuilder

        Dim oDatos As DataSet
        oDatos = New DataSet()
        Dim oParametros As New Dictionary(Of String, Object)

        Try

            'Valida que los folios no estén asociados a una OP
            Dim ErrorMsg As String 'FCRUZ_GMX-10290_INCIDENCIAS  
            Dim ErrorCode As Integer
            ErrorMsg = ""
            Dim oDatosErr As DataSet

            For Each row As DataRow In oGrdOrden.Rows
                oDatosErr = New DataSet

                oParametros = New Dictionary(Of String, Object)
                oParametros.Add("Folio_Onbase", row("FolioOnbase").ToString())
                oParametros.Add("Num_Pago", row("NumeroPago").ToString())
                oParametros.Add("PagarA", Me.cmbTipoUsuario.SelectedValue)

                oDatosErr = Funciones.ObtenerDatos("usp_valida_folio_con_OP", oParametros)

                ErrorCode = oDatosErr.Tables(0).Rows(0).Item("ID_ERROR")

                If ErrorCode <> 0 Then
                    ErrorMsg = ErrorMsg + oDatosErr.Tables(0).Rows(0).Item("MSG_ERROR") + "<br>"
                End If
            Next

            If ErrorMsg <> "" Then
                Mensaje.MuestraMensaje("OrdenPagoSiniestros", ErrorMsg, TipoMsg.Falla)
                Return
            End If

            If drDependencias.Visible = True And drDependencias.SelectedValue.ToString() = "-1" Then
                Mensaje.MuestraMensaje("OrdenPagoSiniestros", "Debe seleccionar una dependencia", TipoMsg.Advertencia)
                Return
            End If

            If ValidarImpuestosOPFac() = True Then
                oSolicitudPago = New StringBuilder
                oImpuestos = New StringBuilder

                If Not GenerarXMLSolicitudPago(oSolicitudPago) Then
                    Mensaje.MuestraMensaje("OrdenPagoSiniestros", "Error al preparar la solicitud de pago", TipoMsg.Falla)
                Else

                    If cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then

                        If Not GenerarXMLImpuestos(oImpuestos) Then
                            Mensaje.MuestraMensaje("OrdenPagoSiniestros", "Error al preparar detalles de impuestos", TipoMsg.Falla)
                            Return
                        End If

                        'FJCP 10290 MEJORAS Validar moneda de la cuenta bancaria que tenga el Proveedor ini
                        If Not ValidaMonedaPagoVSBancaria() Then
                            Mensaje.MuestraMensaje("OrdenPagoSiniestros", "La moneda de pago vs la moneda de la cuenta bancaria habilitada es diferente", TipoMsg.Falla)
                            Return
                        End If
                        'FJCP 10290 MEJORAS Validar moneda de la cuenta bancaria que tenga el Proveedor fin

                    End If




                    oParametros = New Dictionary(Of String, Object)

                    oParametros.Add("SolicitudPago", oSolicitudPago.ToString)
                    oParametros.Add("Impuestos", IIf(cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor, oImpuestos.ToString, DBNull.Value))

                    oDatos = Funciones.ObtenerDatos("usp_CrearSolicitudPago_stro", oParametros)

                    If oDatos Is Nothing OrElse oDatos.Tables.Count = 0 Then
                        Mensaje.MuestraMensaje("OrdenPagoSiniestros", "Error al generar la orden de pago", TipoMsg.Falla)
                    End If

                    If oDatos.Tables(oDatos.Tables.Count - 1).Rows.Count > 0 Then

                        If oDatos.Tables(oDatos.Tables.Count - 1).Columns.Contains("SolicitudPago") AndAlso
                            oDatos.Tables(oDatos.Tables.Count - 1).Columns.Contains("OrdenPago") Then

                            If CInt(oDatos.Tables(oDatos.Tables.Count - 1).Rows(0).Item("SolicitudPago")) = -1 OrElse
                                CInt(oDatos.Tables(oDatos.Tables.Count - 1).Rows(0).Item("OrdenPago")) = -1 Then

                                Dim errorcuentacontable As String
                                If cmbTipoPagoOP.SelectedValue = "T" Then
                                    If oDatos.Tables(1).Rows.Count > 0 Then
                                        errorcuentacontable = " Dar de alta la cuenta: " + oDatos.Tables(1).Rows(0).Item("cta_cble1").ToString()
                                    Else
                                        errorcuentacontable = ""
                                    End If
                                End If

                                Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("No se pudo generar la orden de pago: {0}",
                                                                                            oDatos.Tables(oDatos.Tables.Count - 1).Rows(0).Item("MensajeError") +
                                                                                            errorcuentacontable), TipoMsg.Falla)

                            Else
                                InicializarValores()
                                'Impresión reporte
                                Dim ws As New ws_Generales.GeneralesClient
                                Dim server As String = ws.ObtieneParametro(Cons.TargetReport)
                                server = Replace(Replace(server, "@Reporte", "OrdenPago"), "@Formato", "PDF") & "&nro_op=@nro_op"
                                server = Replace(server, Cons.ReposSource, Cons.ReposReport)
                                server = Replace(server, "OrdenPago", "OrdenPago_stro")
                                'Funciones.EjecutaFuncion("fn_ImprimirOrden('" & server & "','" & "234777" & "');")
                                Funciones.EjecutaFuncion(String.Format("fn_ImprimirOrden('{0}','{1}');",
                                                                       server,
                                                                       CStr(oDatos.Tables(oDatos.Tables.Count - 1).Rows(0).Item("OrdenPago"))))

                                Mensaje.MuestraMensaje("SINIESTROS", String.Format("Solicitud de pago: {0} \n Orden de pago: {1}",
                                                                                        oDatos.Tables(oDatos.Tables.Count - 1).Rows(0).Item("SolicitudPago"),
                                                                                        oDatos.Tables(oDatos.Tables.Count - 1).Rows(0).Item("OrdenPago")), TipoMsg.Confirma)

                                'FJCP GRABAR INFOR DE PAGO INTERNACIONAL
                                grabarDatosPagoInternacional(oDatos.Tables(oDatos.Tables.Count - 1).Rows(0).Item("OrdenPago"))

                            End If
                        Else
                            Mensaje.MuestraMensaje("OrdenPagoSiniestros", "Error al generar la orden de pago.", TipoMsg.Falla)
                        End If
                    End If
                End If
            Else
                Mensaje.MuestraMensaje("OrdenPagoSiniestros", "Error al generar la orden de pago ValidarImpuestosOPFac.", TipoMsg.Falla)
            End If
        Catch ex As Exception
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", "GenerarOrdenPago error: {0}" + ex.ToString(), TipoMsg.Falla)
        End Try
    End Sub

    Private Sub grabarDatosPagoInternacional(nroOP As Integer)
        Try
            Dim dt As New DataTable
            Dim oParametros As New Dictionary(Of String, Object)
            'Dim datosPI As String
            'Dim arregloPI() As String
            'datosPI =

            dt = Session("dtPI")

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then

                oParametros.Add("nro_OP", nroOP)
                If ValidaVacios(dt.Rows(0)("cod_pais").ToString) Then oParametros.Add("cod_pais", dt.Rows(0)("cod_pais").ToString)
                If ValidaVacios(dt.Rows(0)("banco").ToString) Then oParametros.Add("banco", dt.Rows(0)("banco").ToString)
                If ValidaVacios(dt.Rows(0)("num_banco").ToString) Then oParametros.Add("num_banco", dt.Rows(0)("num_banco").ToString)
                If ValidaVacios(dt.Rows(0)("domicilio").ToString) Then oParametros.Add("domicilio", dt.Rows(0)("domicilio").ToString)
                If ValidaVacios(dt.Rows(0)("cuenta").ToString) Then oParametros.Add("cuenta", dt.Rows(0)("cuenta").ToString) 'FJCP_10290_CC
                If ValidaVacios(dt.Rows(0)("aba_routing").ToString) Then oParametros.Add("aba_routing", dt.Rows(0)("aba_routing").ToString)
                If ValidaVacios(dt.Rows(0)("swift").ToString) Then oParametros.Add("swift", dt.Rows(0)("swift").ToString)
                If ValidaVacios(dt.Rows(0)("transit").ToString) Then oParametros.Add("transit", dt.Rows(0)("transit").ToString)
                If ValidaVacios(dt.Rows(0)("iban").ToString) Then oParametros.Add("iban", dt.Rows(0)("iban").ToString)

                If ValidaVacios(dt.Rows(0)("triangulado").ToString) Then oParametros.Add("triangulado", dt.Rows(0)("triangulado").ToString)
                If ValidaVacios(dt.Rows(0)("banco_triang").ToString) Then oParametros.Add("banco_triang", dt.Rows(0)("banco_triang").ToString)
                If ValidaVacios(dt.Rows(0)("cuenta_triang").ToString) Then oParametros.Add("cuenta_triang", dt.Rows(0)("cuenta_triang").ToString)
                If ValidaVacios(dt.Rows(0)("aba_routing_triang").ToString) Then oParametros.Add("aba_routing_triang", dt.Rows(0)("aba_routing_triang").ToString)

                Funciones.ObtenerDatos("usp_grabar_datos_pago_internacional", oParametros)
            End If

        Catch ex As Exception
            MuestraMensaje("Error", "Error grabar Datos Pago Internacional: " + ex.Message(), TipoMsg.Falla)
        End Try
    End Sub
    Private Function ValidaVacios(texto As String) As Boolean
        Try
            If IsNothing(texto) Then
                Return False
            End If

            If texto.Length = 0 Then
                Return False
            End If

            If texto.Trim = "" Then
                Return False
            End If

            If texto.Length > 0 Then
                Return True
            End If

            Return True
        Catch ex As Exception
            Return False

        End Try
    End Function
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
    Public Sub CargarClasePago(ByVal oRegistro As Control, ByVal iFila As Integer, ByVal sValor As String, ByVal scpto As String)

        Dim oParametros As New Dictionary(Of String, Object)

        Dim cmbConceptoPago As DropDownList
        Dim cmbClasePago As DropDownList

        Dim oDatos As DataSet

        Try

            oParametros = New Dictionary(Of String, Object)

            oDatos = New DataSet

            cmbConceptoPago = New DropDownList
            cmbClasePago = New DropDownList

            cmbConceptoPago = BuscarControlPorClase(oRegistro, "estandar-control concepto_pago")
            cmbClasePago = BuscarControlPorClase(oRegistro, "estandar-control clase_pago")

            If Not cmbConceptoPago Is Nothing Then

                If cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then
                    oParametros.Add("Accion", 3)
                Else
                    oParametros.Add("Accion", 1)
                End If

                oParametros.Add("Cod_Pres", Me.txtCodigoBeneficiario_stro.Text)
                oParametros.Add("cod_cpto", scpto)

                oDatos = Funciones.ObtenerDatos("MIS_sp_op_stro_Consulta_Tradicional", oParametros)

                If Not oDatos Is Nothing AndAlso oDatos.Tables(0).Rows.Count > 0 Then

                    If cmbClasePago.Items.Count > 0 Then
                        cmbClasePago.Items.Clear()
                    End If
                    cmbClasePago.DataSource = oDatos
                    cmbClasePago.DataTextField = "clase_pago"
                    cmbClasePago.DataValueField = "cod_clase_pago"
                    cmbClasePago.DataBind()

                    'If cmbClasePago.SelectedValue = 26 Or cmbClasePago.SelectedValue = 75 Then
                    '    cmbOrigenOP.SelectedValue = 5
                    'Else
                    '    cmbOrigenOP.SelectedValue = 6
                    'End If

                    'voy a ingresat este codigo para cargar la clase de pago en la descripcion de la op
                    txtConceptoOP.Text = ""
                    Dim substroDetalle As Integer = 0
                    For Each oFila In oGrdOrden.Rows

                        If txtConceptoOP.Text.Trim = String.Empty Then
                            'txtConceptoOP.Text = String.Format("{0} {1}", txtConceptoOP.Text.Trim, oFila("Siniestro"))
                            txtConceptoOP.Text = String.Format("{0} {1} {2}", txtConceptoOP.Text.Trim, oFila("Siniestro"), oFila("Subsiniestro")) 'FJCP MEJORAS 10290 DETALLES
                            substroDetalle = CInt(oFila("Subsiniestro"))
                        Else
                            'txtConceptoOP.Text = String.Format("{0}, {1}", txtConceptoOP.Text.Trim, oFila("Siniestro"))
                            If substroDetalle <> CInt(oFila("Subsiniestro")) Then
                                txtConceptoOP.Text = String.Format("{0}, {1}", txtConceptoOP.Text.Trim, oFila("Subsiniestro")) 'FJCP MEJORAS 10290 DETALLES
                            End If
                        End If

                    Next
                    txtConceptoOP.Text = String.Format("{0} {1}", txtConceptoOP.Text.Trim.ToString(), oDatos.Tables(0).Rows(0).Item("clase_pago"))
                    oGrdOrden.Rows(iFila)("ClasePago") = oDatos.Tables(0).Rows(0).Item("cod_clase_pago")

                Else
                    'Mensaje.MuestraMensaje("Orden de pago de siniestros", String.Format("El PROVEEDOR NO TIENE HABILITADO ESTA CLASE DE CLASE DE PAGO", "COD_CLASE_PAGO:" + sValor, "CODIGO DE PROVEEDOR" + Me.txtCodigoBeneficiario_stro.Text), TipoMsg.Advertencia)
                    oGrdOrden.Rows(iFila)("ConceptoPago") = ""
                    cmbConceptoPago.Items.Clear()
                    cmbClasePago.Items.Clear()
                End If

            End If

        Catch ex As Exception
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("Clase de pago no habilitada: {0}", ex.Message), TipoMsg.Falla)
        End Try

    End Sub
    Public Sub CargarConceptosPagodefault(ByVal oRegistro As Control, ByVal iFila As Integer, ByVal sValor As String, ByVal stro_OC As String)

        Dim oParametros As New Dictionary(Of String, Object)

        Dim cmbConceptoPago As DropDownList
        Dim cmbClasePago As DropDownList

        Dim oDatos As DataSet

        Try

            oParametros = New Dictionary(Of String, Object)

            oDatos = New DataSet

            cmbConceptoPago = New DropDownList
            cmbClasePago = New DropDownList

            cmbConceptoPago = BuscarControlPorClase(oRegistro, "estandar-control concepto_pago")
            cmbClasePago = BuscarControlPorClase(oRegistro, "estandar-control clase_pago")

            If Not cmbConceptoPago Is Nothing Then

                If cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then
                    oParametros.Add("Accion", 2)
                Else
                    oParametros.Add("Accion", 1)
                End If
                oParametros.Add("Cod_Pres", Me.txtCodigoBeneficiario_stro.Text)
                oParametros.Add("Cod_cpto", "0")
                oParametros.Add("stro_cerrado_abierto", stro_OC) 'CERO SI ESTA ABIERTO Y 1 SI ESTA CERRADO

                'JLC Mejoras En pago a Asegurados y Terceros -Inicio 
                oParametros.Add("Cod_clase_pago ", cmbClasePago.SelectedValue)
                'JLC Mejoras En pago a Asegurados y Terceros -Fin 

                oDatos = Funciones.ObtenerDatos("MIS_sp_op_stro_Consulta_Tradicional", oParametros)

                If Not oDatos Is Nothing AndAlso oDatos.Tables(0).Rows.Count > 0 Then

                    If cmbConceptoPago.Items.Count > 0 Then
                        cmbConceptoPago.Items.Clear()
                    End If
                    cmbConceptoPago.DataSource = oDatos
                    cmbConceptoPago.DataTextField = "Descripcion"
                    cmbConceptoPago.DataValueField = "Concepto"
                    cmbConceptoPago.DataBind()


                Else
                    'Mensaje.MuestraMensaje("Orden de pago de siniestros", String.Format("El PROVEEDOR NO TIENE HABILITADO ESTA CLASE DE CLASE DE PAGO", "COD_CLASE_PAGO:" + sValor, "CODIGO DE PROVEEDOR" + Me.txtCodigoBeneficiario_stro.Text), TipoMsg.Advertencia)
                    oGrdOrden.Rows(iFila)("ConceptoPago") = ""
                    cmbConceptoPago.Items.Clear()
                    cmbClasePago.Items.Clear()
                End If

            End If

        Catch ex As Exception
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("CargarConceptosPago error: {0}", ex.Message), TipoMsg.Falla)
        End Try

    End Sub
    Public Sub CargarConceptosPago(ByVal oRegistro As Control, ByVal iFila As Integer, ByVal sValor As String)

        Dim oParametros As New Dictionary(Of String, Object)

        Dim cmbConceptoPago As DropDownList
        Dim cmbClasePago As DropDownList

        Dim oDatos As DataSet

        Try

            oParametros = New Dictionary(Of String, Object)

            oDatos = New DataSet

            cmbConceptoPago = New DropDownList
            cmbClasePago = New DropDownList

            cmbConceptoPago = BuscarControlPorClase(oRegistro, "estandar-control concepto_pago")
            cmbClasePago = BuscarControlPorClase(oRegistro, "estandar-control clase_pago")

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

                    If CInt(sValor) = 26 Then
                        cmbOrigenOP.SelectedValue = 5
                    End If

                    If CInt(sValor) = 27 Then
                        cmbOrigenOP.SelectedValue = 6
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
                    'Mensaje.MuestraMensaje("Orden de pago de siniestros", String.Format("El PROVEEDOR NO TIENE HABILITADO ESTA CLASE DE CLASE DE PAGO", "COD_CLASE_PAGO:" + sValor, "CODIGO DE PROVEEDOR" + Me.txtCodigoBeneficiario_stro.Text), TipoMsg.Advertencia)
                    oGrdOrden.Rows(iFila)("ConceptoPago") = ""
                    cmbConceptoPago.Items.Clear()
                    cmbClasePago.Items.Clear()
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
            dt.Columns.Add("Factura", Type.GetType("System.String"))
            dt.Columns.Add("FechaComprobante", Type.GetType("System.String"))
            dt.Columns.Add("NumeroComprobante", Type.GetType("System.String"))

            'Campos generales
            dt.Columns.Add("IdSiniestro", Type.GetType("System.Int32"))
            dt.Columns.Add("IdPersona", Type.GetType("System.Int32"))
            dt.Columns.Add("CodigoAsegurado", Type.GetType("System.Int32"))
            dt.Columns.Add("Siniestro", Type.GetType("System.String"))
            dt.Columns.Add("RFC", Type.GetType("System.String"))
            dt.Columns.Add("Subsiniestro", Type.GetType("System.String"))
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
            dt.Columns.Add("Reserva", Type.GetType("System.Double"))
            dt.Columns.Add("ImportePagos", Type.GetType("System.Double"))

            dt.Columns.Add("CondicionISR", Type.GetType("System.Int32"))
            dt.Columns.Add("CondicionCED", Type.GetType("System.Int32"))
            dt.Columns.Add("MonedaFactura", Type.GetType("System.Int32"))
            'campos de fasttrack
            dt.Columns.Add("FastTrack", Type.GetType("System.String"))
            'campos multipago y numero pago
            dt.Columns.Add("FolioOnbase", Type.GetType("System.Int32")) 'FJCP MULTIPAGO AGREGAR 
            dt.Columns.Add("NumeroPago", Type.GetType("System.Int32")) 'FJCP MEJORAS FASE II NUMERO PAGO


            oGrdOrden = dt

            grd.DataSource = oGrdOrden
            grd.DataBind()

        Catch ex As Exception
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("IniciaGrid error: {0}", ex.Message), TipoMsg.Falla)
        End Try


    End Sub

    Private Sub limpiarCamposTipoUsuario() 'FJCP 10290 MEJORAS
        Me.txtBeneficiario.Text = String.Empty
        Me.txtOnBase.Text = String.Empty
        Me.txtSiniestro.Text = String.Empty
        Me.txtPoliza.Text = String.Empty
        Me.txtMonedaPoliza.Text = String.Empty
        Me.txtRFC.Text = String.Empty
        Me.txtCodigoBeneficiario_stro.Text = String.Empty
        Me.txtBeneficiario_stro.Text = String.Empty

        Me.txtTotalAutorizacion.Text = String.Empty 'importe de la poliza
        Me.txtTotalImpuestos.Text = String.Empty
        Me.txtTotalRetenciones.Text = String.Empty
        Me.txtTotal.Text = String.Empty  'importe de la poliza

        Me.iptxtTotalAutorizacion.Text = String.Empty 'importe de pago
        Me.iptxtTotalImpuestos.Text = String.Empty
        Me.iptxtTotal.Text = String.Empty  'importe de pago

        Me.txtTotalAutorizacionFac.Text = String.Empty 'txt de facturas
        Me.txtTotalImpuestosFac.Text = String.Empty
        Me.txtTotalRetencionesFac.Text = String.Empty
        Me.txtTotalFac.Text = String.Empty
        Me.txtTotalAutorizacionNacionalFac.Text = String.Empty
        Me.txtDescuentos.Text = String.Empty 'txt de facturas

        Me.txtConceptoOP.Text = String.Empty
        Me.oSucursalT_stro.Value = String.Empty
        Me.oBancoT_stro.Value = String.Empty
        Me.oBeneficiarioT_stro.Value = String.Empty
        Me.oCuentaBancariaT_stro.Value = String.Empty
        Me.oMonedaT_stro.Value = String.Empty
        Me.oTipoCuentaT_stro.Value = String.Empty
        Me.oPlazaT_stro.Value = String.Empty
        Me.oAbaT_stro.Value = String.Empty
        'drBeneficiario.
        Dim dt As New DataTable
        drBeneficiario.DataSource = dt
        drBeneficiario.DataBind()

        If Me.cmbSubsiniestro.Items.Count > 0 Then
            Me.cmbSubsiniestro.Items.Clear()
        End If


    End Sub

    Private Sub btnNvoTercero_Click(sender As Object, e As EventArgs) Handles btnNvoTercero.Click
        Try
            Funciones.AbrirModal("#RegistroTerceros")

        Catch ex As Exception
            MuestraMensaje("ex", ex.Message, TipoMsg.Falla)
        End Try
    End Sub

    Private Sub LlenaDDLAsegurado(poliza As String)
        Dim dt As New DataTable
        Dim param As String
        Try
            param = Replace(poliza, "-", ",")

            Funciones.fn_Consulta("SELECT DISTINCT cod_aseg, nombre FROM f_aseg_poliza(" & param & ") ", dt)
            Dim codAseg = Funciones.fn_Ejecuta("SELECT cod_aseg FROM f_aseg_poliza(" & param & ") WHERE tipo_persona = 'Asegurado'")
            Funciones.LlenaDDL(drBeneficiario, dt, "cod_aseg", "nombre", 0, False)
            drBeneficiario.SelectedValue = codAseg

        Catch ex As Exception
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("El endoso no tiene ubicaciones: (" & param & ")", ex.Message), TipoMsg.Falla)
            Limpiartodo()
        End Try
    End Sub

    Private Sub habilitarCampos() 'FJCP 10290 MEJORAS 
        If cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then
            Me.cmbTipoPagoOP.SelectedValue = "T"
            Me.btnVerCuentas.Visible = True
            Me.txtSiniestro.Enabled = False

            Me.txtBeneficiario_stro.Visible = True
            Me.drBeneficiario.Visible = False
            Me.txtBeneficiario_stro.Width = System.Web.UI.WebControls.Unit.Percentage(110)
            Me.lblNvoTercero.Visible = False
            Me.btnNvoTercero.Visible = False
            Me.drBeneficiario.Visible = False
            cmbNumPago.Enabled = False  'FJCP MEJORAS FASE II NUMERO PAGO


        ElseIf cmbTipoUsuario.SelectedValue = eTipoUsuario.Tercero Then
            Me.txtBeneficiario_stro.Visible = True
            Me.txtBeneficiario_stro.Width = System.Web.UI.WebControls.Unit.Percentage(100)
            Me.lblNvoTercero.Visible = True
            Me.btnNvoTercero.Visible = True
            Me.drBeneficiario.Visible = False

            Me.cmbTipoPagoOP.SelectedValue = "T"
            Me.btnVerCuentas.Visible = False
            Me.txtSiniestro.Enabled = True
            cmbNumPago.Enabled = True  'FJCP MEJORAS FASE II NUMERO PAGO

        ElseIf cmbTipoUsuario.SelectedValue = eTipoUsuario.Asegurado Then
            Me.drBeneficiario.Visible = True
            Me.txtBeneficiario_stro.Width = System.Web.UI.WebControls.Unit.Percentage(110)
            Me.txtBeneficiario_stro.Visible = False

            Me.lblNvoTercero.Visible = False
            Me.btnNvoTercero.Visible = False

            Me.cmbTipoPagoOP.SelectedValue = "T"
            Me.btnVerCuentas.Visible = False
            Me.txtSiniestro.Enabled = True
            cmbNumPago.Enabled = True  'FJCP MEJORAS FASE II NUMERO PAGO
        End If
    End Sub
#End Region
    'JLC Mejoras Tipo de Cambio Pactado - Inicio 
    Protected Sub txt_fecha_ini_TextChanged(sender As Object, e As EventArgs) Handles txt_fecha_ini.TextChanged
        Dim oDatos As DataSet

        Dim oParametros As New Dictionary(Of String, Object)
        Try
            oParametros = New Dictionary(Of String, Object)




            'oParametros.Add("TipoMoneda", Me.cmbMonedaPago.SelectedValue)
            oParametros.Add("TipoMoneda", 1)
            oParametros.Add("Fecha", Funciones.FormatearFecha(txt_fecha_ini.Text, Funciones.enumFormatoFecha.YYYYMMDD)
                                                             )

            oDatos = New DataSet

            oDatos = Funciones.ObtenerDatos("usp_ObtenerCambio_stro", oParametros)

            If Not oDatos Is Nothing AndAlso oDatos.Tables(0).Rows.Count > 0 Then
                txt_tipoCambioConsultado.Text = oDatos.Tables(0).Rows(0).Item(2)
            Else
                txt_tipoCambioConsultado.Text = 1
            End If


        Catch ex As Exception

            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("ObtenerTipoCambio error: {0}", ex.Message), TipoMsg.Falla)
        End Try
    End Sub
    ' //JLC Mejoras Tipo de Cambio Pactado - Fin 


    ' //JLC Mejoras Fecha Estimada de pago - Inicio 
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
    ' //JLC Mejoras Fecha Estimada de pago - Fin 

    Private Sub drBeneficiario_SelectedIndexChanged(sender As Object, e As EventArgs) Handles drBeneficiario.SelectedIndexChanged
        Me.txtCodigoBeneficiario_stro.Text = drBeneficiario.SelectedValue.ToString()
        Me.txtBeneficiario.Text = drBeneficiario.SelectedItem.ToString()
        Me.txtBeneficiario_stro.Text = drBeneficiario.SelectedItem.ToString()
    End Sub



    Private Function ValidaMonedaPagoVSBancaria() As Boolean
        Try
            Dim monedaPago As Integer
            Dim monedaPagoBancaria As Integer


            monedaPago = cmbMonedaPago.SelectedValue
            monedaPagoBancaria = oMonedaT_stro.Value


            If monedaPago <> monedaPagoBancaria Then
                Return False
            End If

            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function



    Public Function ObtenerDatosTransferenciaDepGob() As Boolean

        Dim oParametros As New Dictionary(Of String, Object)
        Dim ctaParam As String
        Dim oDatos As DataSet
        Dim dt As New DataTable

        Try

            If grd.Rows.Count > 0 AndAlso cmbTipoPagoOP.SelectedValue = "T" Then

                oDatos = New DataSet

                oParametros = New Dictionary(Of String, Object)

                'oParametros.Add("Codigo", CInt(oGrdOrden.Rows(0).Item("IdPersona")))
                oParametros.Add("Accion", 1)
                oParametros.Add("Codigo", CInt(txtCodigoBeneficiario_stro.Text.Trim))
                oParametros.Add("PagarA", cmbTipoUsuario.SelectedValue)

                oDatos = Funciones.ObtenerDatos("usp_CargarDatosBancariosDepGob_stro", oParametros)

                If Not oDatos Is Nothing AndAlso oDatos.Tables(0).Rows.Count > 0 Then

                    With oDatos.Tables(0).Rows(0)

                        oBancoT_stro.Value = .Item("CodigoBanco")
                        oMonedaT_stro.Value = cmbMonedaPago.SelectedValue
                        oTipoCuentaT_stro.Value = 2
                        oCuentaBancariaT_stro.Value = .Item("NumeroCuenta")
                        'oBeneficiarioT_stro.Value = .Item("Beneficiario")

                    End With





                Else
                    drDependencias.Visible = False
                    lblDependencias.Visible = False
                    'If Me.cmbTipoUsuario.SelectedValue <> eTipoUsuario.Proveedor Then
                    'Mensaje.MuestraMensaje("Cuentas bancarias", "No existen cuentas asociadas", TipoMsg.Falla)
                    'Me.cmbTipoPagoOP.SelectedValue = "C"
                    'Me.btnVerCuentas.Visible = False
                    'End If
                End If

                oSucursalT_stro.Value = "CIUDAD DE MEXICO"
                oBeneficiarioT_stro.Value = IIf(oBeneficiarioT_stro.Value = String.Empty, Me.txtBeneficiario.Text.Trim, oBeneficiarioT_stro.Value)

            End If

            ctaParam = Funciones.fn_EjecutaStr("usp_CargarDatosBancariosDepGob_stro @Accion = 2")

            If oCuentaBancariaT_stro.Value = ctaParam Then
                drDependencias.Visible = True
                lblDependencias.Visible = True
                Funciones.fn_Consulta("usp_CargarDatosBancariosDepGob_stro @Accion = 3, @Codigo = " + txtCodigoBeneficiario_stro.Text.Trim + ", @PagarA = " + cmbTipoUsuario.SelectedValue.ToString(), dt)
                Funciones.LlenaDDL(drDependencias, dt, "codigo", "txt_desc", 0, False)
                Return False
            End If

            Return True
        Catch ex As Exception
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("ObtenerDatosTransferenciaProveedor error: {0}", ex.Message), TipoMsg.Falla)
            Return False
        End Try

    End Function

    Private Sub drDependencias_SelectedIndexChanged(sender As Object, e As EventArgs) Handles drDependencias.SelectedIndexChanged
        Dim codDep As String
        Dim cpto2 As String
        codDep = drDependencias.SelectedValue.ToString()

        If drDependencias.SelectedValue = "-1" Then
            MuestraMensaje("Dependencias", "Seleccione una dependencia", TipoMsg.Advertencia)
            txtcpto2.Text = ""
        Else
            cpto2 = Funciones.fn_EjecutaStr("usp_CargarDatosBancariosDepGob_stro @Accion = 4, @depen = '" + codDep.ToString + "'")
            txtcpto2.Text = cpto2
        End If
    End Sub

    Private Function validaFolioBloqueado() As Boolean
        Dim oDatos As DataSet
        Dim oParametros As New Dictionary(Of String, Object)
        Try
            hidCodUsuario.Value = Master.cod_usuario

            oParametros.Add("Accion", 1)
            oParametros.Add("folioOnbase", Me.txtOnBase.Text.Trim)
            oParametros.Add("cod_usuario", Master.cod_usuario)

            Funciones.ObtenerDatos("usp_bloqueoFolioOnbase_stro", oParametros)
            'VZAVALETA_GMX-10290_INCIDENCIAS
            oParametros = New Dictionary(Of String, Object)

            oParametros.Add("Accion", 4)
            oParametros.Add("folioOnbase", Me.txtOnBase.Text.Trim)
            oParametros.Add("cod_usuario", Master.cod_usuario)

            oDatos = Funciones.ObtenerDatos("usp_bloqueoFolioOnbase_stro", oParametros)

            If Not oDatos Is Nothing AndAlso oDatos.Tables(0).Rows.Count > 0 Then
                If oDatos.Tables(0).Rows(0).Item("Folio_Onbase").ToString().Length > 0 Then
                    Mensaje.MuestraMensaje("Folio OnBase Bloqueado", "El Folio: " + Me.txtOnBase.Text.Trim + " está siendo utilizado en el proceso automático, se encuentra en el Lote: " + oDatos.Tables(0).Rows(0).Item("Num_lote").ToString(), TipoMsg.Falla)

                    Limpiartodo()
                    Return True
                End If
            End If
            'VZAVALETA_GMX-10290_INCIDENCIAS
            'oDatos = New DataSet
            oParametros = New Dictionary(Of String, Object)

            oParametros.Add("Accion", 2)
            oParametros.Add("folioOnbase", Me.txtOnBase.Text.Trim)
            oParametros.Add("cod_usuario", Master.cod_usuario)

            oDatos = Funciones.ObtenerDatos("usp_bloqueoFolioOnbase_stro", oParametros)

            If Not oDatos Is Nothing AndAlso oDatos.Tables(0).Rows.Count > 0 Then
                If oDatos.Tables(0).Rows(0).Item("cod_usuario_bloqueo").ToString().Length > 0 Then
                    Mensaje.MuestraMensaje("Folio OnBase Bloqueado", "El Folio: " + Me.txtOnBase.Text.Trim + " está siendo utilizado por el usuario: " + oDatos.Tables(0).Rows(0).Item("cod_usuario_bloqueo").ToString(), TipoMsg.Falla)
                    Limpiartodo()
                    Return True
                End If
            End If

            Return False
        Catch ex As Exception
            Return False
        End Try

    End Function


    Public Function BuscarFolioOnbase() As Boolean
        Dim sElemento As String = String.Empty
        Dim oDatos As DataSet
        Dim oParametros As New Dictionary(Of String, Object)


        Try

            Dim nPAgo As Integer = IIf(cmbNumPago.SelectedValue.ToString() = "", 1, cmbNumPago.SelectedValue)
            'FJCP 10290 MEJORAS Pagar A ini
            Salir = 1
            If Not ObtenerPagarA(Me.txtOnBase.Text, nPAgo) Then
                If Salir = 0 Then
                    Exit Function
                End If
                MuestraMensaje("Pagar", "No existe dato para campo Pagar A", TipoMsg.Falla)
                'Exit Sub
                Exit Function
            End If
            'FJCP 10290 MEJORAS Pagar A fin


            Dim PosibleDescuento As Decimal
            'FJCP MULTIPAGO COMENTAR SECCION INI
            If chkVariasFacturas.Checked = False Then
                Dim chkdelete As CheckBox
                For Each row In grd.Rows
                    chkdelete = BuscarControlPorID(row, "eliminar")
                    chkdelete.Checked = True
                Next
                EliminarFila(1)
            End If
            'FJCP MULTIPAGO COMENTAR SECCION FIN

            'FJCP 10290 MEJORAS Folio OnBase ini
            Dim hrefOnBase As String
            linkOnBase.HRef = ""
            hrefOnBase = ""
            'hrefOnBase = Funciones.fn_EjecutaStr("usp_consulta_folio_onbase_ws  @id_pagar_a = " & Me.cmbTipoUsuario.SelectedValue & ",  @folioOnbase = " & txtOnBase.Text.Trim & ", @num_pago = " & cmbNumPago.SelectedValue)

            hrefOnBase = Funciones.fn_EjecutaStr("usp_consulta_folio_onbase_ws  @id_pagar_a = " & Me.cmbTipoUsuario.SelectedValue & ",  @folioOnbase = " & txtOnBase.Text.Trim & ", @num_pago = " & nPAgo)

            linkOnBase.HRef = hrefOnBase

            If validaFolioBloqueado() Then
                'Exit Sub
                Exit Function
            End If

            'FJCP 10290 MEJORAS Folio OnBase fin


            Select Case Me.cmbTipoUsuario.SelectedValue

                Case eTipoUsuario.Proveedor


                    oParametros.Add("Folio_OnBase", Me.txtOnBase.Text.Trim)
                    oParametros.Add("id_PagarA", Me.cmbTipoUsuario.SelectedValue) 'VZAVALETA_10290_CC

                    oDatos = Funciones.ObtenerDatos("sp_op_stro_consulta_folio_OnBase", oParametros)

                    If Not oDatos Is Nothing AndAlso oDatos.Tables(0).Rows.Count > 0 Then

                        oSeleccionActual = oDatos.Tables(0)




                        With oDatos.Tables(0).Rows(0)

                            If (oDatos.Tables(0).Rows(0).Item("sn_relacionado") = "-1") Then
                                'Mensaje.MuestraMensaje("Folio OnBase Relacionado", "Fecha Relacionado: " + oDatos.Tables(0).Rows(0).Item("fecha_relacion").ToString() + " Usuario relacion: " + oDatos.Tables(0).Rows(0).Item("cod_usuario_relacion").ToString() + " Fecha de Comprobante: " + oDatos.Tables(0).Rows(0).Item("fecha_emision_gmx").ToString(), TipoMsg.Falla)
                                Mensaje.MuestraMensaje("Folio OnBase Relacionado", "Fecha Relacionado: " + oDatos.Tables(0).Rows(0).Item("fecha_relacion").ToString() + "<br>" + "Usuario relacion: " + oDatos.Tables(0).Rows(0).Item("cod_usuario_relacion").ToString() + "<br>" + "Fecha de Comprobante: " + oDatos.Tables(0).Rows(0).Item("fecha_emision_gmx").ToString() + "<br>" + "OP Relacionada: " + oDatos.Tables(0).Rows(0).Item("Nro_OP").ToString(), TipoMsg.Falla) 'FJCP 10290 MEJORAS Folio OnBase Relacionado
                                Limpiartodo()
                            Else
                                If (oDatos.Tables(0).Rows(0).Item("fec_fact") = "1") Then
                                    Me.txtOnBase.Text = .Item("num_folio")
                                    Me.txtSiniestro.Text = .Item("num_siniestro")
                                    Me.txtRFC.Text = .Item("RFC")
                                    Me.txtPoliza.Text = .Item("poliza")
                                    Me.txtMonedaPoliza.Text = .Item("txt_desc")
                                    Me.txtNumeroComprobante.Text = .Item("folio_GMX")
                                    Me.txtFechaComprobante.Text = .Item("fecha_emision_gmx")

                                    Me.txtBeneficiario.Text = .Item("Proveedor")
                                    Me.txtBeneficiario_stro.Text = .Item("Proveedor")
                                    Me.txtCodigoBeneficiario_stro.Text = .Item("cod_pres")

                                    Me.txtTipoCambio.Text = IIf(Me.txtMonedaPoliza.Text = "NACIONAL", "1.00", IIf(.Item("cod_moneda") = 0, "1.00", ObtenerTipoCambio.ToString()))

                                    'Mostrar los importes de la factura de conta electronica
                                    Me.txtTotalAutorizacionNacionalFac.Text = String.Format("{0:0,0.00}", Math.Round(.Item("imp_subtotal"), 2))
                                    Me.txtTotalAutorizacionFac.Text = String.Format("{0:0,0.00}", Math.Round(.Item("imp_subtotal"), 2))
                                    Me.txtTotalImpuestosFac.Text = String.Format("{0:0,0.00}", Math.Round(.Item("imp_impuestos"), 2))
                                    Me.txtTotalRetencionesFac.Text = String.Format("{0:0,0.00}", Math.Round(.Item("imp_retencion"), 2))
                                    Me.txtTotalFac.Text = String.Format("{0:0,0.00}", Math.Round(.Item("imp_total"), 2))
                                    'Si la moneda de la factura es nacional y la de la póliza es extranjera
                                    'se asignara el tipo de cambio como nacional, por lo tanto solo se podrá pagar en 
                                    PosibleDescuento = Decimal.Parse(.Item("imp_subtotal")) + Decimal.Parse(.Item("imp_impuestos")) - Decimal.Parse(.Item("imp_retencion"))
                                    PosibleDescuento = Decimal.Parse(PosibleDescuento) - Decimal.Parse(.Item("imp_total"))
                                    If Math.Abs(PosibleDescuento) > 0.9 Then
                                        lbldescuento.Text = "Factura con Posible descuento de: " + PosibleDescuento.ToString()
                                    Else
                                        lbldescuento.Text = ""
                                    End If
                                    'JLC Mejoras Moneda Pago-Inicio
                                    If .Item("cod_moneda") = 0 Then
                                        If .Item("Moneda_Hara_Pago") = 1 Then
                                            Mensaje.MuestraMensaje("Moneda", "No puedes pagar en dolares por que la moneda de la factura esta en pesos: ", TipoMsg.Falla)
                                            ' Limpiartodo()'se comenta por que solo se requiere una alerta
                                        Else
                                            cmbMonedaPago.SelectedValue = 0
                                        End If
                                    Else
                                        cmbMonedaPago.SelectedValue = 1
                                    End If
                                    If .Item("Moneda_Hara_Pago") = 0 Then
                                        If .Item("cod_moneda") = 1 Then
                                            Mensaje.MuestraMensaje("Moneda", "No puedes pagar en pesos por que la moneda de la factura esta en dolares: ", TipoMsg.Falla)
                                            ' Limpiartodo()'se comenta por que solo se requiere una alerta
                                        Else
                                            cmbMonedaPago.SelectedValue = 0
                                        End If
                                    Else
                                        cmbMonedaPago.SelectedValue = 1
                                    End If
                                    'JLC Mejoras Moneda Pago-Fin
                                    'se limpian las cajas de impuestos 

                                    txtTotalAutorizacion.Text = 00.00
                                    txtTotalImpuestos.Text = 00.00
                                    txtTotalRetenciones.Text = 00.00
                                    txtTotal.Text = 00.00
                                    txtTotalNacional.Text = 00.00

                                    iptxtTotalAutorizacion.Text = 00.00
                                    iptxtTotalImpuestos.Text = 00.00
                                    iptxtTotalRetenciones.Text = 00.00
                                    iptxtTotal.Text = 00.00
                                    iptxtTotalNacional.Text = 00.00

                                    If .Item("Moneda_Hara_Pago") = 1 Then
                                        Me.cmbMonedaPago.SelectedValue = 1
                                    Else
                                        Me.cmbMonedaPago.SelectedValue = 0
                                    End If


                                    'moneda nacional.
                                    If .Item("cod_moneda") = 0 And Not Me.txtMonedaPoliza.Text = "NACIONAL" Then
                                        'Mensaje.MuestraMensaje("Calculo de totales", "Factura capturada en pesos, se utilizará tipo de cambio nacional.", TipoMsg.Advertencia)
                                        'Me.txtTipoCambio.Text = "1.00"
                                        Me.cmbMonedaPago.SelectedValue = 0
                                    End If
                                    If txtMonedaPoliza.Text = cmbMonedaPago.SelectedItem.ToString() Then  'FJCP 10290_CC
                                        txtTipoCambio.Text = "1"
                                    Else
                                        Me.txtTipoCambio.Text = IIf(Me.txtMonedaPoliza.Text = "NACIONAL", "1.00", ObtenerTipoCambio.ToString())
                                    End If
                                    'Se agrega por el tema de 4 campos mas
                                    If .Item("sn_transferencia") <> .Item("Forma_Hara_Pago") Then
                                        Mensaje.MuestraMensaje("Moneda", "No coincide la forma del pago MIS vs SIIGMX (OP WEB) ", TipoMsg.Falla)
                                        Limpiartodo() 'VZAVALETA_GMX-10290_INCIDENCIAS
                                    End If
                                    If .Item("sn_transferencia") = -1 Then
                                        Me.cmbTipoPagoOP.SelectedValue = "T"
                                    Else
                                        If .Item("sn_transferencia") = 0 Then
                                            Me.cmbTipoPagoOP.SelectedValue = "C"
                                        End If
                                    End If

                                    Me.txt_clase.Text = .Item("cod_clase_pago")
                                Else
                                    Mensaje.MuestraMensaje("Fecha Comprobante menor al año fiscal: ", "Fecha del comprobante Fiscal: " + oDatos.Tables(0).Rows(0).Item("fecha_emision_gmx").ToString(), TipoMsg.Falla)
                                    Limpiartodo()
                                End If
                            End If
                        End With

                        oClavesPago = IIf(oDatos.Tables(1) Is Nothing OrElse oDatos.Tables(1).Rows.Count = 0, Nothing, oDatos.Tables(1))

                        If Not oDatos.Tables(2) Is Nothing AndAlso oDatos.Tables(2).Rows.Count > 0 Then

                            oOrigenesPago = IIf(oOrigenesPago Is Nothing OrElse oOrigenesPago.Rows.Count = 0, oDatos.Tables(2), oOrigenesPago)
                            cmbOrigenOP.Items.Clear()
                            For Each fila In oDatos.Tables(2).Rows
                                Me.cmbOrigenOP.Items.Add(New ListItem(fila.Item("DescripcionOrigenPago").ToString.ToUpper, fila.Item("CodigoOrigenPago")))
                            Next

                        End If
                        cmbSubsiniestro.Items.Clear()
                        For Each fila In oDatos.Tables(0).Rows
                            Me.cmbSubsiniestro.Items.Add(New ListItem(String.Format("Subsiniestro {0}", fila.Item("id_substro")).ToUpper, fila.Item("id_substro")))
                        Next
                        'FJCP 10290 MEJORAS Seleccionar Subsiniestro
                        If Not oDatos.Tables(0).Rows(0).Item("Subsiniestro") Is Nothing AndAlso oDatos.Tables(0).Rows(0).Item("Subsiniestro") <> "VARIOS" Then
                            cmbSubsiniestro.SelectedValue = oDatos.Tables(0).Rows(0).Item("Subsiniestro")
                        End If

                        Me.lblObBase.Visible = True
                        Me.txtOnBase.Visible = True

                        cmbTipoComprobante.Items.Clear()
                        If cmbTipoComprobante.Items.Count = 0 Then

                            cmbTipoComprobante.DataSource = oDatos.Tables(3)
                            cmbTipoComprobante.DataTextField = "Descripcion"
                            cmbTipoComprobante.DataValueField = "CodigoComprobante"
                            cmbTipoComprobante.DataBind()

                            'cmbTipoComprobante.SelectedValue = oDatos.Tables(0).Rows(0).Item("Id_Tipo_Doc")
                        End If

                        'Onbase.Style("display") = ""
                        pnlProveedor.Style("display") = ""

                    Else
                        If Not oDatos Is Nothing AndAlso oDatos.Tables(4).Rows.Count > 0 Then
                            'FFUENTES Esto es en caso de que no traiga nada la consulta con todas las tablas esta solo es la tabla de factura_conta_electronica 
                            oSeleccionActual = oDatos.Tables(4)
                            With oDatos.Tables(4).Rows(0)
                                If (oDatos.Tables(4).Rows(0).Item("sn_relacionado") = "-1") Then
                                    'Mensaje.MuestraMensaje("Folio OnBase Relacionado", "Fecha Relacionado: " + oDatos.Tables(4).Rows(0).Item("fecha_relacion").ToString() + " Usuario relacion: " + oDatos.Tables(4).Rows(0).Item("cod_usuario_relacion").ToString() + " Fecha de Comprobante: " + oDatos.Tables(4).Rows(0).Item("fecha_emision_gmx").ToString(), TipoMsg.Falla)
                                    Mensaje.MuestraMensaje("Folio OnBase Relacionado", "Fecha Relacionado: " + oDatos.Tables(4).Rows(0).Item("fecha_relacion").ToString() + "<br>" + "Usuario relacion: " + oDatos.Tables(4).Rows(0).Item("cod_usuario_relacion").ToString() + "<br>" + "Fecha de Comprobante: " + oDatos.Tables(4).Rows(0).Item("fecha_emision_gmx").ToString() + "<br>" + "OP Relacionada: " + oDatos.Tables(4).Rows(0).Item("Nro_OP").ToString(), TipoMsg.Falla) 'FJCP 10290 MEJORAS Folio OnBase Relacionado
                                    Limpiartodo()
                                Else
                                    Mensaje.MuestraMensaje("Folio Onbase con datos erroneos:", "Folio Onbase: " + oDatos.Tables(4).Rows(0).Item("num_folio").ToString() + " Numero Siniestro: " + oDatos.Tables(4).Rows(0).Item("num_siniestro").ToString() + " RFC Proveedor: " + oDatos.Tables(4).Rows(0).Item("RFC_proveedor").ToString(), TipoMsg.Falla)

                                    'Me.txtPoliza.Text = .Item("poliza")
                                    'Me.txtMonedaPoliza.Text = .Item("txt_desc")
                                    'Me.txtNumeroComprobante.Text = .Item("folio_GMX")
                                    'Me.txtFechaComprobante.Text = .Item("fecha_emision_gmx")
                                    Limpiartodo()
                                    EliminarFila(1)
                                    'Me.txtBeneficiario.Text = .Item("Proveedor")
                                    'Me.txtBeneficiario_stro.Text = .Item("Proveedor")
                                    'Me.txtCodigoBeneficiario_stro.Text = .Item("cod_pres")
                                End If
                            End With
                        Else
                            oSeleccionActual = Nothing
                            Me.txtOnBase.Text = String.Empty
                            Me.txtSiniestro.Text = String.Empty
                            Me.txtRFC.Text = String.Empty
                            Me.txtPoliza.Text = String.Empty
                            Me.txtMonedaPoliza.Text = String.Empty
                        End If
                    End If
                Case Else
                    oParametros.Add("Folio_OnBase", Me.txtOnBase.Text.Trim)
                    'oParametros.Add("numPago", cmbNumPago.SelectedValue) 'FJCP MEJORAS FASE II NUMERO PAGO
                    oParametros.Add("numPago", IIf(cmbNumPago.SelectedValue.ToString() = "", 1, cmbNumPago.SelectedValue))
                    oParametros.Add("id_PagarA", Me.cmbTipoUsuario.SelectedValue) 'VZAVALETA_10290_CC

                    oDatos = Funciones.ObtenerDatos("sp_op_stro_consulta_folio_OnBase", oParametros)
                    If Not oDatos Is Nothing AndAlso oDatos.Tables(0).Rows.Count > 0 Then
                        oSeleccionActual = oDatos.Tables(0)
                        With oDatos.Tables(0).Rows(0)
                            'VZAVALETA_10290_CC_INICIO
                            If (oDatos.Tables(0).Rows(0).Item("sn_relacionado") = "-1") Then
                                Mensaje.MuestraMensaje("Folio OnBase Relacionado", "Fecha Relacionado: " + oDatos.Tables(0).Rows(0).Item("fecha_relacion").ToString() + "<br>" + "Usuario relacion: " + oDatos.Tables(0).Rows(0).Item("cod_usuario_relacion").ToString() + "<br>" + "Fecha de Comprobante: " + oDatos.Tables(0).Rows(0).Item("fecha_emision_gmx").ToString() + "<br>" + "OP Relacionada: " + oDatos.Tables(0).Rows(0).Item("Nro_OP").ToString(), TipoMsg.Falla) 'FJCP 10290 MEJORAS Folio OnBase Relacionado
                                Limpiartodo()
                            Else
                                'If (oDatos.Tables(0).Rows(0).Item("fec_fact") = "1") Then
                                Me.txtSiniestro.Text = .Item("nro_stro")
                                Me.txt_clase.Text = .Item("cod_clase_pago")
                                '     Else
                                ' Mensaje.MuestraMensaje("Fecha Comprobante menor al año fiscal: ", "Fecha del comprobante Fiscal: " + oDatos.Tables(0).Rows(0).Item("fecha_emision_gmx").ToString(), TipoMsg.Falla)
                                ' Limpiartodo()
                                'End If
                            End If
                            'VZAVALETA_10290_CC_FIN
                        End With
                        'valida los datos de numero de siniestro
                        oParametros.Clear()
                        oParametros.Add("Numero_Siniestro", Me.txtSiniestro.Text.Trim)
                        oParametros.Add("FolioOnbase", Me.txtOnBase.Text.Trim)
                        oParametros.Add("numPago", IIf(cmbNumPago.SelectedValue.ToString() = "", 1, cmbNumPago.SelectedValue)) 'FJCP MEJORAS FASE II NUMERO PAGO

                        oDatos = Funciones.ObtenerDatos("sp_op_stro_consulta_numero_siniestro", oParametros)

                        Me.cmbSubsiniestro.Items.Clear()
                        Me.cmbOrigenOP.Items.Clear()

                        If Not oDatos Is Nothing AndAlso oDatos.Tables(0).Rows.Count > 0 Then

                            oSeleccionActual = oDatos.Tables(0)

                            With oDatos.Tables(0).Rows(0)
                                Me.txtSiniestro.Text = .Item("nro_stro")
                                Me.txtRFC.Text = IIf(Me.cmbTipoUsuario.SelectedValue = eTipoUsuario.Asegurado, .Item("RFC"), String.Empty)
                                Me.txtPoliza.Text = .Item("poliza")
                                Me.txtMonedaPoliza.Text = .Item("txt_desc")
                                Me.txtBeneficiario.Text = String.Format("{0} {1} {2}", .Item("txt_apellido1"), .Item("txt_apellido2"), .Item("txt_nombre")).ToUpper

                                If Me.cmbTipoUsuario.SelectedValue = eTipoUsuario.Asegurado Then
                                    Me.txtBeneficiario_stro.Text = Me.txtBeneficiario.Text.Trim
                                    Me.txtCodigoBeneficiario_stro.Text = .Item("cod_aseg")
                                    'FJCP 10290 MEJORAS Nombre y Razón Social - Asegurado
                                    LlenaDDLAsegurado(Me.txtPoliza.Text.Trim)
                                    txtBeneficiario_stro.Visible = False
                                    drBeneficiario.Visible = True
                                    cmbNumPago.Enabled = True
                                Else
                                    Me.txtBeneficiario_stro.Text = String.Empty
                                    Me.txtCodigoBeneficiario_stro.Text = String.Empty
                                    Me.txtBeneficiario.Text = String.Empty
                                    'FJCP 10290 MEJORAS Nombre y Razón Social - Asegurado
                                    txtBeneficiario_stro.Visible = True
                                    drBeneficiario.Visible = False
                                    txtRFC.Enabled = True
                                    txtBeneficiario_stro.Enabled = True
                                    txtNumeroComprobante.Enabled = True
                                    txtFechaComprobante.Enabled = False

                                    If Me.cmbTipoUsuario.SelectedValue = eTipoUsuario.Proveedor Then 'FJCP MEJORAS FASE II NUMERO DE PAGO
                                        cmbNumPago.Enabled = False
                                    Else
                                        cmbNumPago.Enabled = True
                                    End If
                                End If



                                'se agrego por el tema de los 4 campos mas 
                                'If .Item("Moneda_poliza") = 0 Then
                                '    'If .Item("cod_moneda") = 1 Then se agrega por el tema de los 4 campos mas
                                '    If .Item("Moneda_Hara_Pago") = 1 Then
                                '        Mensaje.MuestraMensaje("Moneda", "No puedes pagar en dolares por que la moneda de la Poliza esta en pesos: ", TipoMsg.Falla)
                                '        ' Limpiartodo()'se comenta por que solo se requiere una alerta
                                '    Else
                                '        cmbMonedaPago.SelectedValue = 0
                                '    End If
                                'Else
                                '    cmbMonedaPago.SelectedValue = 1
                                'End If
                                'JJARAMILLO 10290 Se comento la parte superior por choque en moneda_poliza y se agrego la condicion inferior
                                If .Item("Moneda_Hara_Pago") = 1 Then
                                    cmbMonedaPago.SelectedValue = 1
                                Else
                                    cmbMonedaPago.SelectedValue = 0
                                End If

                                If .Item("Forma_Hara_Pago") = -1 Then
                                    Me.cmbTipoPagoOP.SelectedValue = "T"
                                Else
                                    If .Item("Forma_Hara_Pago") = 0 Then
                                        Me.cmbTipoPagoOP.SelectedValue = "C"
                                    End If
                                End If
                            End With

                            oClavesPago = IIf(oDatos.Tables(1) Is Nothing OrElse oDatos.Tables(1).Rows.Count = 0, Nothing, oDatos.Tables(1))

                            If Not oDatos.Tables(2) Is Nothing AndAlso oDatos.Tables(2).Rows.Count > 0 Then

                                oOrigenesPago = IIf(oOrigenesPago Is Nothing OrElse oOrigenesPago.Rows.Count = 0, oDatos.Tables(2), oOrigenesPago)

                                If Me.cmbOrigenOP.Items.Count > 0 Then
                                    Me.cmbOrigenOP.Items.Clear()
                                End If

                                For Each fila In oDatos.Tables(2).Rows
                                    Me.cmbOrigenOP.Items.Add(New ListItem(fila.Item("DescripcionOrigenPago").ToString.ToUpper, fila.Item("CodigoOrigenPago")))
                                Next

                            End If

                            For Each fila In oDatos.Tables(0).Rows
                                Me.cmbSubsiniestro.Items.Add(New ListItem(String.Format("Subsiniestro {0}", fila.Item("id_substro")).ToUpper, fila.Item("id_substro")))
                            Next
                            'FJCP 10290 MEJORAS Seleccionar Subsiniestro
                            If Not oDatos.Tables(0).Rows(0).Item("Subsiniestro") Is Nothing AndAlso oDatos.Tables(0).Rows(0).Item("Subsiniestro") <> "VARIOS" Then
                                cmbSubsiniestro.SelectedValue = oDatos.Tables(0).Rows(0).Item("Subsiniestro")
                            End If
                            'CARGO LOS TIPOS DE CODUMENTOS PARA ASEGURADOS Y TERCEROS
                            cmbTipoComprobante.Items.Clear()
                            If cmbTipoComprobante.Items.Count = 0 Then

                                cmbTipoComprobante.DataSource = oDatos.Tables(4)
                                cmbTipoComprobante.DataTextField = "Descripcion_Doc"
                                cmbTipoComprobante.DataValueField = "Id_Tipo_Doc"
                                cmbTipoComprobante.DataBind()

                                cmbTipoComprobante.SelectedValue = oDatos.Tables(0).Rows(0).Item("Id_Tipo_Doc")

                            End If

                            If txtMonedaPoliza.Text = cmbMonedaPago.SelectedItem.ToString() Then  'FJCP 10290_CC
                                txtTipoCambio.Text = "1"
                            Else
                                Me.txtTipoCambio.Text = IIf(Me.txtMonedaPoliza.Text = "NACIONAL", "1.00", ObtenerTipoCambio.ToString())
                            End If



                        Else

                            oSeleccionActual = Nothing

                            Me.txtOnBase.Text = String.Empty
                            Me.txtSiniestro.Text = String.Empty
                            Me.txtRFC.Text = String.Empty
                            Me.txtPoliza.Text = String.Empty
                            Me.txtMonedaPoliza.Text = String.Empty
                        End If

                        'Onbase.Style("display") = "none" 'FFUENTES none
                        'pnlProveedor.Style("display") = "none"
                        pnlProveedor.Style("display") = "" 'FCJP MULTIPAGO
                    Else
                        'ESTO SE AGREGA PARA VER QUIEN TIENE RELACIONADO EL FOLIO ONBASE DE ASEGURADOS O TERCEROS
                        If (oDatos.Tables(4).Rows(0).Item("sn_relacionado") = "-1") Then
                            'Mensaje.MuestraMensaje("Folio OnBase Relacionado", "Fecha Relacionado: " + oDatos.Tables(4).Rows(0).Item("fecha_relacion").ToString() + " Usuario relacion: " + oDatos.Tables(4).Rows(0).Item("cod_usuario_relacion").ToString() + " Fecha de Comprobante: " + oDatos.Tables(4).Rows(0).Item("fecha_emision_gmx").ToString(), TipoMsg.Falla)
                            Mensaje.MuestraMensaje("Folio OnBase Relacionado", "Fecha Relacionado: " + oDatos.Tables(4).Rows(0).Item("fecha_relacion").ToString() + "<br>" + "Usuario relacion: " + oDatos.Tables(4).Rows(0).Item("cod_usuario_relacion").ToString() + "<br>" + "Fecha de Comprobante: " + oDatos.Tables(4).Rows(0).Item("fecha_emision_gmx").ToString() + "<br>" + "OP Relacionada: " + oDatos.Tables(4).Rows(0).Item("Nro_OP").ToString(), TipoMsg.Falla) 'FJCP 10290 MEJORAS Folio OnBase Relacionado
                            Limpiartodo()
                            ''ESTO LO COMENTE
                            'Else
                            '    Mensaje.MuestraMensaje("Folio Onbase con datos erroneos:", "Folio Onbase: " + oDatos.Tables(4).Rows(0).Item("num_folio").ToString() + " Numero Siniestro: " + oDatos.Tables(4).Rows(0).Item("num_siniestro").ToString() + " RFC Proveedor: " + oDatos.Tables(4).Rows(0).Item("RFC_proveedor").ToString(), TipoMsg.Falla)

                            '    Limpiartodo()
                            '    EliminarFila(1)
                        End If
                    End If
            End Select

            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function


    Private Function validaNumeroPago() As Boolean 'FJCP MEJORAS FASE II NUMERO PAGO
        Dim oDatos As DataSet
        Dim oParametros As New Dictionary(Of String, Object)
        Dim dt As DataTable
        Try


            Dim dt1 As New DataTable
            cmbNumPago.DataSource = dt1
            cmbNumPago.DataBind()


            oParametros.Add("Accion", 1)
            oParametros.Add("folioOnbase", Me.txtOnBase.Text.Trim)



            oDatos = Funciones.ObtenerDatos("usp_numPago_FolioOnbase_stro", oParametros)

            dt = oDatos.Tables(0)
            Funciones.LlenaDDL(cmbNumPago, dt, "cod_numeroPago", "numeroPago", 0, False)
            If Not dt Is Nothing And dt.Rows.Count > 1 Then
                'FJCP 10290 MEJORAS Pagar A ini
                ObtenerPagarA(Me.txtOnBase.Text, cmbNumPago.SelectedValue)
                Dim dtt As New DataTable

                dtt = agregaSeleccionar(dt)
                Funciones.LlenaDDL(cmbNumPago, dtt, "cod_numeroPago", "numeroPago", 0, False)

                Mensaje.MuestraMensaje("Numero de Pago", "Seleccione el número de Pago", TipoMsg.Advertencia)
                Return False
            Else
                BuscarFolioOnbase()
            End If
            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function

    Private Sub cmbNumPago_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbNumPago.SelectedIndexChanged 'FJCP MEJORAS FASE II NUMERO PAGO
        Try
            If cmbNumPago.SelectedValue <> -1 Then
                cmbTipoUsuario.SelectedValue = eTipoUsuario.Ninguno
                BuscarFolioOnbase()
            End If

        Catch ex As Exception

        End Try
    End Sub






    Private Function agregaSeleccionar(dt As DataTable) As DataTable 'FJCP MEJORAS FASE II NUMERO PAGO
        Try
            Dim dtN As New DataTable

            For Each col As DataColumn In dt.Columns
                dtN.Columns.Add(col.ColumnName, col.DataType)
            Next
            dtN.Rows.Add(-1, "...")
            dtN.Merge(dt)

            Return dtN
        Catch ex As Exception
            Mensaje.MuestraMensaje("Exepcion", ex.Message, TipoMsg.Falla)
            Return Nothing
        End Try
    End Function

    Private Sub btnTipoCambio_Click(sender As Object, e As EventArgs) Handles btnTipoCambio.Click
        Try
            If Valida_Tipo_Pago() Then
                Funciones.AbrirModal("#Modal")
            Else
                Mensaje.MuestraMensaje("Tipo de Cambio", "La moneda de pago y de la póliza, no permiten modificar el tipo de cambio.", TipoMsg.Advertencia)
            End If
            'txtTipoCambio.Enabled = True
            'txt_tipoCambioConsultado.Enabled = True
        Catch ex As Exception
        End Try
    End Sub

    Private Function Valida_Tipo_Pago() As Boolean
        Try
            'VZAVALETA_10290_CC INI
            'Cuando la moneda de pago y de la póliza sea pesos el tipo de cambio sea 1 y debe estar bloqueado
            'Cuando la moneda de pago y de la póliza sea dólares tipo de cambio sea 1 y debe estar bloqueado
            'Cuando la moneda de pago  sea pesos y de la póliza sea dólares  se puede seleccionar tipo de cambio 
            'Cuando la moneda de pago  sea dólares y de la póliza sea pesos  se puede seleccionar tipo de cambio 
            If txtMonedaPoliza.Text = "NACIONAL" Then
                If cmbMonedaPago.SelectedValue = 0 Then
                    'Mensaje.MuestraMensaje("Tipo de Cambio", "La moneda de pago y de la póliza, no permiten modificar el tipo de cambio.", TipoMsg.Advertencia)
                    txtTipoCambio.Text = 1
                    txtTipoCambio.Enabled = False
                    txt_tipoCambioConsultado.Enabled = False
                    Return False
                End If
            End If

            If txtMonedaPoliza.Text = "DOLAR AMERICANO" Then
                If cmbMonedaPago.SelectedValue = 1 Then
                    ' Mensaje.MuestraMensaje("Tipo de Cambio", "La moneda de pago y de la póliza, no permiten modificar el tipo de cambio.", TipoMsg.Advertencia)
                    txtTipoCambio.Text = 1
                    txtTipoCambio.Enabled = False
                    txt_tipoCambioConsultado.Enabled = False
                    Return False
                End If
            End If

            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function

End Class
