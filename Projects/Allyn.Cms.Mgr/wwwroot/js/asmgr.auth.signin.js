(function (ts) {
    //提示消息插件配置
    ts.options = {
        closeButton: false, //是否显示关闭按钮
        debug: false, //是否使用debug模式
        positionClass: "toast-bottom-right",//弹出窗的位置
        showDuration: "300",//显示的动画时间
        hideDuration: "1000",//消失的动画时间
        timeOut: "3000", //展现时间
        extendedTimeOut: "1000",//加长展示时间
        showEasing: "swing",//显示时的动画缓冲方式
        hideEasing: "linear",//消失时的动画缓冲方式
        showMethod: "fadeIn",//显示时的动画方式
        hideMethod: "fadeOut" //消失时的动画方式
    };
}(toastr));
(function (win) {
    //视图数据模型
    win.viewModel = function (verifyToken, form) {
        var self = this;

        //获取Url中的参数
        self.getParam = function (name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
            var r = location.search.substr(1).match(reg);
            return r == null ? null : unescape(decodeURI(r[2]));
        }

        //防伪标识
        self.verifyToken = ko.observable(verifyToken);

        //表单验证
        self.validator = form.bootstrapValidator({
            message: '此值没有通过验证',
            feedbackIcons: {
                valid: 'glyphicon glyphicon-ok',
                invalid: 'glyphicon glyphicon-remove',
                validating: 'glyphicon glyphicon-refresh'
            },
            fields: {
                account: {
                    validators: {
                        notEmpty: {
                            message: '账号不能为空'
                        },
                        regexp: {
                            regexp: /^[a-zA-Z0-9]{5,10}$/,
                            message: '账号只能包含5~10个字母或数字'
                        }
                    }
                },
                password: {
                    validators: {
                        notEmpty: {
                            message: '密码不能为空'
                        },
                        stringLength: {
                            min: 6,
                            max: 18,
                            message: '账号长度必须在6到18位之间'
                        }
                    }
                }
            }
        });

        self.model = {
            name: ko.observable(),
            password: ko.observable(),
            rememberMe: ko.observable(true),
            __RequestVerificationToken: self.verifyToken
        };

        self.signInClick = function (e) {
            if (self.validator.data('bootstrapValidator').isValid()) {
                $.ajax({
                    type: 'post',
                    url: 'https://rest.allyn.com.cn/Auth/SignIn',
                    data: ko.mapping.toJSON(self.model),
                    contentType: 'application/json; charset=utf-8',
                    success: function (token) {
                        $.ajax({
                            type: 'post',
                            url: '/auth/signinasync',
                            data: { 'token': token },
                            success: function () {
                                var returnUrl = self.getParam('ReturnUrl');
                                location.href = returnUrl ? returnUrl:'/';
                            },
                            error: function (xhr, ts, et) {
                                toastr.error(xhr.responseText ? xhr.responseText : xhr.status + '(' + xhr.statusText + ')', '错误提示');
                            }
                        });
                    },
                    error: function (xhr, ts, et) {
                        toastr.error(xhr.responseText ? xhr.responseText : xhr.status + '(' + xhr.statusText + ')', '错误提示');
                    }
                });
            } else {
                self.validator.data("bootstrapValidator").validate();
            }
        }
    }
}(window));