Imports Microsoft.VisualBasic

Public Class Cons

    'PARAMETROS A DONDE APUNTA LA APLICACION CPAR_PARAMETROS
    Public Const TargetReport = 9 '9=UAT,8=PRODUCCION,3=DESA
    Public Const ReposSource = "ReportesGMX_UAT"
    Public Const ReposReport = "UAT_ReportesOPSiniestros"
    Public Const TargetFiltrosAdminStro = 30
    Public Const TargetMenuAdminStro = 31
    Public Const RutaWebServOnBase = 50
    Public Const FechaFiltroStrosTec = 51
    Public Const ConsultaExportExcel = 52
    Public Const UsuariosNicho1 = 53
    '---------------------------------------------------------

    'Valores Reportes (cREP_Reportes)
    Public Const ModuloRea As Integer = 16
    Public Const ModuloStrosAdmon As Integer = 7
    Public Const ModuloStrosTec As Integer = 71

    Public Const SubModRecSin As Integer = 41
    Public Const RptFac As Integer = 3
    Public Const RptEsp As Integer = 4

    'Parametros (cPAR_Parametros)
    Public Const ParamRpt As Integer = 3
    Public Const ConsulFac As Integer = 5
    Public Const ConsulEsp As Integer = 6

    Public Const ReaseguroProduccion = 4
    Public Const ReaseguroPruebas = 10
    Public Const ReaseguroLocal = 23

    Public Const ReportesDESA = 3
    Public Const ReportesProduccion = 8
    Public Const ReportesUAT = 9

    Public Const MachoteAcuseOP = 11
    Public Const Rol5801 = 12
    Public Const Rol5802 = 13
    Public Const Rol5803 = 14
    Public Const Rol5804 = 15
    Public Const Rol5805 = 16
    Public Const Rol5806 = 17
    Public Const MachoteAvisoProrroga = 18

    Public Const FirmasProduccion = 19
    Public Const FirmasPruebas = 24

    Public Const Repositorio = 26

    Public Const SectorReaseguro = 32

    Public Const StrosTradicional = 1
    Public Const StrosFondos = 3

    Public Const SubModFirmasFondos = 94
    Public Const SubModCancelacion = 98


    Public Const SubModMesaControl As Integer = 11
    Public Const SubModTableroControl As Integer = 12
    Public Const SubModMemoriaCalculo As Integer = 13
    Public Const SubModFirmas As Integer = 53
    Public Const SubModAcusePago As Integer = 54
    Public Const SubModOrdenPago As Integer = 55
    Public Const SubModReporteador As Integer = 71
    Public Const SubModRepDetCtaCte As Integer = 72
    Public Const SubModRepDecenal As Integer = 75
    Public Const SubModContactos As Integer = 81
    Public Const SubModReSumenTemp As Integer = 311
    Public Const SubModRepGarantias As Integer = 561
    Public Const SubModRepCalendario As Integer = 562
    Public Const SubModBitacora As Integer = 563
    Public Const SubModBitacoraFirma As Integer = 564

    'Tipo de Filtro Firmas
    Public Enum TipoFiltro
        Todas = 0
        PorRevisar = 1
        Pendientes = 2
        Firmadas = 3
        Rechazadas = 4
        Autorizadas = 5
        Revisadas = 6
    End Enum
    Public Enum TipoFiltroN
        Todas = 0
        Pendientes = 2
        SinFirma = 7
        Rechazadas = 4
        Autorizadas = 5
        NoProc = 6
    End Enum

    'Tipo Consulta
    Public Enum TipoMov
        Fac = 1
        Esp = 2
    End Enum

    'Acciones Filtros
    Public Enum Accion
        Consultar
        Guardar
    End Enum

    'Modulos Recordatorios
    Public Enum Recordatorio
        Siniestros = 61
    End Enum

    Public Enum TipoPersona
        Manual = -1
        Solicitante = 5800
        JefeInmediato = 5801
        Subdirector = 5802
        Director = 5803
        Tesoreria = 5804
        Contabilidad = 5805
        Rechazo = 5806
        DirectorGeneral = 5807
    End Enum
End Class
