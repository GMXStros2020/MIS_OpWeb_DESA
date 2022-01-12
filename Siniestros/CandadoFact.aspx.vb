
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

        Leercandado()
    End Sub

#End Region

#Region "Funciones"

#End Region

#Region "Métodos"
    Public Sub Leercandado()
        Dim oDatos As DataSet
        oDatos = New DataSet()
        Dim oParametros As New Dictionary(Of String, Object)

        oParametros.Add("Accion", 3)

        oDatos = Funciones.ObtenerDatos("CandadoFacturacion", oParametros)

        If oDatos Is Nothing OrElse oDatos.Tables.Count = 0 Then
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", " ", TipoMsg.Falla)
        Else
            If oDatos.Tables(0).Rows(0).Item("sn_parametro") = "0" Then
                lblObBase.Text = "DESACTIVADO: " + oDatos.Tables(0).Rows(0).Item("sn_parametro").ToString()
            Else
                lblObBase.Text = "ACTIVO: " + oDatos.Tables(0).Rows(0).Item("sn_parametro").ToString()
            End If
        End If
    End Sub
    Public Sub ActivarCandado() Handles btnAct_Candado.Click
        Try
            Dim oDatos As DataSet
            oDatos = New DataSet()
            Dim oParametros As New Dictionary(Of String, Object)

            oParametros.Add("Accion", 2)

            oDatos = Funciones.ObtenerDatos("CandadoFacturacion", oParametros)

            If oDatos Is Nothing OrElse oDatos.Tables.Count = 0 Then
                Mensaje.MuestraMensaje("OrdenPagoSiniestros", " ", TipoMsg.Falla)
            Else
                Mensaje.MuestraMensaje("OrdenPagoSiniestros", "Candado Activo", TipoMsg.Falla)
                Leercandado()
            End If

        Catch ex As Exception
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", "GenerarOrdenPago error: {0}" + ex.ToString(), TipoMsg.Falla)
        End Try
    End Sub

#End Region

    Public Sub DesactivarCandado() Handles btnDes_Candado.Click
        Try
            Dim oDatos As DataSet
            oDatos = New DataSet()
            Dim oParametros As New Dictionary(Of String, Object)

            oParametros.Add("Accion", 1)

            oDatos = Funciones.ObtenerDatos("CandadoFacturacion", oParametros)

            If oDatos Is Nothing OrElse oDatos.Tables.Count = 0 Then
                Mensaje.MuestraMensaje("OrdenPagoSiniestros", " ", TipoMsg.Falla)
            Else
                Mensaje.MuestraMensaje("OrdenPagoSiniestros", "Candado Desactivado", TipoMsg.Falla)
                Leercandado()
            End If

        Catch ex As Exception
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", "GenerarOrdenPago error: {0}" + ex.ToString(), TipoMsg.Falla)
        End Try
    End Sub

End Class
