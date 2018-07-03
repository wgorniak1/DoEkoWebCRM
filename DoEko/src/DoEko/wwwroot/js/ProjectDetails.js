$(document).ready(function () {


});

class elementFactory{
    static createDiv(id,classes){
        var div = document.createElement('div');
        div.className = classes;
        div.id = id;
        return div;
    }
}


//<div class="modal fade" id="SrvCancelModal" tabindex="-1" role="dialog" aria-labelledby="SrvCancelModalTitle">
//    <div class="modal-dialog" role="document">
//        <div class="modal-content">
//            <div class="modal-header">
//                <button type="button" class="close srvcancelmodal-btn-cancel" data-dismiss="modal" aria-label="Close">
//                    <span aria-hidden="true">&times;</span>
//                </button>
//                <h4 class="modal-title" id="SrvCancelModalTitle">Anulowanie ankiety</h4>
//            </div>
//            <div class="modal-body">
//                @Html.Partial("Survey/_SurveyCancelForm", new DoEko.ViewModels.SurveyViewModels.SurveyCancelViewModel())
//            </div>
//            <div class="modal-footer">
//                <button type="button" class="btn btn-danger srvcancelmodal-btn-cancel" data-dismiss="modal">Anuluj</button>
//                <button type="button" class="btn btn-success srvcancelmodal-btn-submit">Potwierdź</button>
//            </div>
//        </div>
//    </div>
//</div>

class options{
    var root;
    var header;
    var body;
    var footer;

    constructor(){
        root = new root();
        header = new header();
        body = new body();
        footer = new footer();
    }
}

class root{
    var id = '';
    var className = '';
}
class header{

    }
class body{

    }
class footer{

    }

class myModal{

    constructor(){
    }
    static modalCount;

    static mergeOptions(options){
        
        options.root = {
            className: 'modal fade ' + options.root.className,
            id: options.root.Id || 'ModalWindowNo' + (++modalCount).toString(),

        }
    }

    static createModalForm(options){
        var modalClass = "modal fade";
        var modalTitleId = options.modalId + 'Title';

        //options.modalId
        //options.className
        //



        if (options.className !== undefined) {
            modalClass = modalClass + ' ' + options.className;
        }
        var  root = elementFactory.createDiv(options.modalId,modalClass,"dialog");

        root.setAttribute("tabindex","-1");
        root.setAttribute("aria-labelledby",modalTitleId);

    }

}

class RSEPriceSettings {

    constructor (){

        //Register event handler
        $('button[data-trigger=""]').click(function(){ show($(this).data('project')); });
    }

    show(projectId){
        
        getComponent(projectId);

        
    }

    static GetInstance() {
        var a = new RSEPriceSettings();
    }
}