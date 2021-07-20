<%@ WebHandler Language="VB" Class="Prueba" %>

Imports System
Imports System.Web
Imports System.Data
Imports System.Web.Script.Serialization
Imports Newtonsoft.Json

Public Class Prueba : Implements IHttpHandler

    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        context.Response.ContentType = "text/plain"


        Dim oDatos As DataSet
        Dim oTabla As DataTable
        Dim oParametros As New Dictionary(Of String, Object)
        Dim serializer As New JavaScriptSerializer
        Dim Catalgo As String
        Dim TipoUsuario As String
        Dim id_tipo_pago As String
        Dim CodigoPres As String
        Dim codAnalista As String
        Dim FolioOnbase As String

        Catalgo = context.Request.QueryString("Catalgo")

        Dim OP As New OrdenPagoMasivoClass

        serializer.MaxJsonLength = 500000000

        Select Case Catalgo
            Case "ClasePago"

                oParametros.Add("strCatalogo", "ClasePago")
                oDatos = Funciones.ObtenerDatos("[sp_Catalogos_OPMasivas]", oParametros)
                oTabla = oDatos.Tables(0)
            Case "ConceptoPago"

                TipoUsuario = context.Request.QueryString("TipoUsuario")
                id_tipo_pago = context.Request.QueryString("id_tipo_pago")
                CodigoPres = context.Request.QueryString("CodigoPres")


                Select Case TipoUsuario
                    Case "Asegurado"
                        TipoUsuario = "7"
                    Case "Tercero"
                        TipoUsuario = "8"
                    Case "Proveedor"
                        TipoUsuario = "10"
                End Select


                oParametros.Add("TipoUsuario", TipoUsuario)
                oParametros.Add("id_tipo_pago", id_tipo_pago)
                If String.IsNullOrEmpty(CodigoPres) = False Then
                    oParametros.Add("CodigoPres", CodigoPres)
                End If



                oDatos = Funciones.ObtenerDatos("usp_ObtenerConceptosPagoMasivo_stro", oParametros)
                oTabla = oDatos.Tables(0)
            Case "ConceptoPagoFondos"

                TipoUsuario = context.Request.QueryString("TipoUsuario")
                id_tipo_pago = context.Request.QueryString("id_tipo_pago")
                CodigoPres = context.Request.QueryString("CodigoPres")
                codAnalista = context.Request.QueryString("codAnalista")


                Select Case TipoUsuario
                    Case "Asegurado"
                        TipoUsuario = "7"
                    Case "Tercero"
                        TipoUsuario = "8"
                    Case "Proveedor"
                        TipoUsuario = "10"
                End Select


                oParametros.Add("TipoUsuario", TipoUsuario)
                oParametros.Add("Fondos", "1")
                oParametros.Add("cod_analista", codAnalista)

                If TipoUsuario = "10" Then
                    If String.IsNullOrEmpty(CodigoPres) = False Then
                        oParametros.Add("CodigoPres", CodigoPres)
                    End If
                End If


                oDatos = Funciones.ObtenerDatos("usp_ObtenerConceptosPagoMasivo_stro", oParametros)
                oTabla = oDatos.Tables(0)

            Case "ClasePagoFondos"
                FolioOnbase = context.Request.QueryString("FolioOnbase")
                TipoUsuario = context.Request.QueryString("TipoUsuario")

                Select Case TipoUsuario
                    Case "Asegurado"
                        TipoUsuario = "7"
                    Case "Tercero"
                        TipoUsuario = "8"
                    Case "Proveedor"
                        TipoUsuario = "10"
                End Select

                oParametros.Add("Folio_OnBase", FolioOnbase)
                oParametros.Add("Accion", "2")
                oParametros.Add("paga_a", TipoUsuario)
                oDatos = Funciones.ObtenerDatos("MIS_sp_cir_op_stro_Catalogos_Fondos2", oParametros)
                oTabla = oDatos.Tables(1)

            'Case "CptoPagoFondosProv"
            '    'CodigoPres = context.Request.QueryString("CodigoPres")
            '    oParametros.Add("Fondos", "1")
            '    oParametros.Add("TipoUsuario", 10)
            '    'If String.IsNullOrEmpty(CodigoPres) = False Then
            '    '    oParametros.Add("CodigoPres", CodigoPres)
            '    'End If
            '    oDatos = Funciones.ObtenerDatos("usp_ObtenerConceptosPagoMasivo_stro", oParametros)
            '    oTabla = oDatos.Tables(0)

            'Case "CptoPagoFondosAse"
            '    oParametros.Add("Fondos", "1")
            '    oParametros.Add("TipoUsuario", 8)
            '    oDatos = Funciones.ObtenerDatos("usp_ObtenerConceptosPagoMasivo_stro", oParametros)
            '    oTabla = oDatos.Tables(0)

            Case "FigurasPoliza"
                Dim param As String
                Dim poliza As String
                poliza = context.Request.QueryString("Poliza")
                param = Replace(poliza, "-", ",")

                Funciones.fn_Consulta("SELECT DISTINCT cod_aseg, nombre FROM f_aseg_poliza(" & param & ") ", oTabla)
        End Select


        context.Response.Write(JsonConvert.SerializeObject(oTabla))
    End Sub

    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class