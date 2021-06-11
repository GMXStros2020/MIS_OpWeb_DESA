//jQuery(document).ready(function () {

//    jQuery('#modGeneraCartas').on('hidden.bs.modal', function (e) {
//        jQuery(this).removeData('bs.modal');
//        jQuery(this).find('.modal-content').empty();
//    })

//})
////////////////////////////////////////////////////////////////////EVENTO EXPANDIR-CONTRAER/////////////////////////////////////
//Colapsar Ventana
$("body").on("click", ".contraer", function () {
    event.preventDefault ? event.preventDefault() : event.returnValue = false;
    var id = this.id.substr(this.id.length - 1)
    fn_CambiaEstado(id, "1");
});

//Expandir Ventana
$("body").on("click", ".expandir", function () {
    event.preventDefault ? event.preventDefault() : event.returnValue = false;
    var id = this.id.substr(this.id.length - 1)
    fn_CambiaEstado(id, "0");
});


//Ramo (producto)
$("body").on("keydown", "[id$=txt_SearchProd]", function (e) {
    if (e.which == 9) {
        $("input[id$='txt_ClaveProdu']").focus();
        return false;
    }
    fn_Autocompletar("Age", "txt_ClaveProdu", "txt_SearchProd", "", 3, e.which)
});
//function fn_Autocompletar(Catalogo, ControlClave, ControlBusqueda, Condicion, minChar, caracter, CatalogoAux, ControlClaveAux, ControlBusquedaAux, CondicionAux, blnMultiple)

$("body").on("focusout", "[id$=txt_SearchProd]", function () {
    if ($(this)[0].value == '') {
        $("input[id$='txt_ClaveProdu']")[0].value = ''
    }
    fn_EvaluaAutoComplete('txt_ClaveProdu', 'txt_SearchProd');
});

$("body").on("focus", "[id$=txt_SearchProd]", function () {
    fn_Autocompletar("Age", "txt_ClaveProdu", "txt_SearchProd", "", 3, -1)
    $(this).trigger({
        type: "keydown",
        which: 46
    });
});

//Asegurado
$("body").on("keydown", "[id$=txt_SearchAseg]", function (e) {
    fn_Autocompletar("Ase", "txt_ClaveAseg", "txt_SearchAseg", "", 3, e.which)
});

$("body").on("focusout", "[id$=txt_SearchAseg]", function (e) {
    fn_EvaluaAutoComplete('txt_ClaveAseg', 'txt_SearchAseg');
});

$("body").on("focus", "[id$=txt_SearchAseg]", function () {
    fn_Autocompletar("Age", "txt_ClaveAseg", "txt_SearchAseg", "", 3, -1)
    $(this).trigger({
        type: "keydown",
        which: 46
    });
});


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

function soloNumeros(e) {
    var key;
    if (window.event) // IE
    {
        key = e.keyCode;
    }
    else if (e.which) // Netscape/Firefox/Opera
    {
        key = e.which;
    }

    if (key < 48 || key > 57) {
        return false;
    }
    return true;
}


function convMayusculas(control) {
    var str = $("[id*=" + control + "]").val();
    var strMayus = str.toUpperCase();
    $("[id*=" + control + "]").val(strMayus);
}


//function LengthCheck() {

//    var dato = $("[id*=NombreControl]").val();
//    var long = dato.length;

//    if (long >= 10) {
//        fn_MuestraMensaje('dato', "Ha alcanzado el numero de caracteres permitidos", 2);
//    }


//};

//function enabAutoriza() {
//    $("[id*=btnSolAut]").removeAttr('disabled');
//    //$("[id*=btnSolAut]").removeAttr("disabled");
//}
