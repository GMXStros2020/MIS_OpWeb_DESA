<%@ Page Title="" Language="VB" MasterPageFile="~/Pages/SiteMaster.master" AutoEventWireup="false" CodeFile="CandadoFact.aspx.vb" Inherits="Siniestros_OrdenPago" %>
<%@ MasterType VirtualPath="~/Pages/SiteMaster.master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cph_principal" runat="Server">
    <asp:HiddenField runat="server" ID="hid_Ventanas" Value="1|0" />
    <script src="../Scripts/Siniestros/OrdenPagoFondos.js"></script>
    <style>
        .table>tbody>tr>td{
            padding: 0px;
        }

        .table>tbody>tr>td>input, .table>tbody>tr>td>select{
            border: 0px;
        }

        .chkeliminar{
            border: 0px;
        }

        .table>tbody>tr>th{
            padding: 2px;
        }

        .zona-principal{
            padding-right: 10px;
        }

        .btnEjemplo{
            margin-top: 20px;
        }

        @media (max-width:1080px){
            
            .zona-fechas{
               display:none;
            }

            .zona-form{
                padding-left:20px;
            }
        }

    </style>
    <div class="zona-principal" style="overflow-x: hidden; overflow-y: hidden">

        <asp:UpdatePanel runat="server" ID="upGenerales">
            <ContentTemplate>
                <div class="cuadro-titulo panel-encabezado">
                    <input type="image" src="../Images/contraer_mini_inv.png" id="coVentana1" class="contraer" />
                    <input type="image" src="../Images/expander_mini_inv.png" id="exVentana1" class="expandir" />
                     Candado Facturacion / <asp:Label ID="lblTitulo" runat="server" Text=""></asp:Label>
                </div>
                <asp:HiddenField runat="server" ID="hid_Operacion" Value="0" />
                <div class="panel-contenido ventana1">                    
                    <div style="width:100%; text-align: left">
                                <asp:LinkButton ID="btnAct_Candado" runat="server" class="btn btn-primary btn-sm" style="background-color: #006AA9;">
                                    <span>
                                        <img class="btn-guardar"/> Aplicar Candado
                                    </span>
                                </asp:LinkButton>
                            
                                <asp:LinkButton ID="btnDes_Candado" runat="server" class="btn btn-primary btn-sm" style="background-color: #006AA9;">
                                    <span>
                                        <img class="btn-guardar"/> Desactivar Candado
                                    </span>
                                </asp:LinkButton>
                        <asp:Label ID="lblCandadoO" runat="server" class="etiqueta-control">El candado esta: </asp:Label>
                        <asp:Label ID="lblObBase" runat="server" class="etiqueta-control">Folio Onbase</asp:Label>
                     </div>                    
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <br /><br />
    </div>

</asp:Content>
