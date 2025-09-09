(function () {
    function bindIndexButtons() {
        const area = document.getElementById('ajax-area');
        const btnNova = document.getElementById('btnNova');
        if (btnNova) {
            btnNova.addEventListener('click', function () {
                const url = this.getAttribute('data-url');
                loadForm(url, area);
            });
        }
        document.querySelectorAll('.js-edit').forEach(function (btn) {
            btn.addEventListener('click', function () {
                const url = this.getAttribute('data-url');
                loadForm(url, area);
            });
        });
    }

    function loadForm(url, container) {
        fetch(url, { credentials: 'same-origin' })
            .then(r => r.text())
            .then(html => {
                container.innerHTML = html;
                bindForm(container);
            })
            .catch(() => {
                container.innerHTML = '<div class="alert alert-danger">Falha ao carregar o formulário.</div>';
            });
    }

    function bindForm(container) {
        const form = container.querySelector('#form-noticia');

        if (window.jQuery && window.jQuery.validator && window.jQuery.validator.unobtrusive && form) {
            const $form = window.jQuery(form);
            $form.removeData('validator');
            $form.removeData('unobtrusiveValidation');
            window.jQuery.validator.unobtrusive.parse($form);
        }

        const btnCancelar = container.querySelector('#btnCancelarForm');
        if (!form) return;

        if (btnCancelar) {
            btnCancelar.addEventListener('click', function () {
                container.innerHTML = '';
            });
        }

        form.addEventListener('submit', function (e) {
            e.preventDefault();

            if (window.jQuery && window.jQuery.fn.validate) {
                const $f = window.jQuery(form);
                if (!$f.valid()) return;
            }

            const tokenEl = form.querySelector('input[name="__RequestVerificationToken"]');
            const data = collectFormData(form);
            const url = form.getAttribute('action');

            fetch(url, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json; charset=utf-8',
                    'RequestVerificationToken': tokenEl ? tokenEl.value : ''
                },
                body: JSON.stringify(data),
                credentials: 'same-origin'
            })
                .then(async res => {
                    const payload = await res.json().catch(() => ({}));
                    if (!res.ok || !payload.ok) {
                        const msg = (payload && payload.error) ? payload.error : 'Não foi possível salvar.';
                        showError(container, msg);
                        return;
                    }
                    window.location.href = '/noticias';
                })
                .catch(() => showError(container, 'Falha de comunicação.'));
        });
    }

    function collectFormData(form) {
        const id = parseInt(form.querySelector('#Id')?.value || '0');
        const titulo = form.querySelector('#Titulo')?.value || '';
        const texto = form.querySelector('#Texto')?.value || '';
        const usuarioId = parseInt(form.querySelector('#UsuarioId')?.value || '1');

        let tagIds = [];
        const sel = form.querySelector('#SelectedTagIds');
        if (sel && sel.tagName === 'SELECT' && sel.multiple) {
            tagIds = Array.from(sel.selectedOptions || [])
                .map(o => parseInt(o.value))
                .filter(n => !isNaN(n));
        } else {
            tagIds = Array.from(form.querySelectorAll('input[name="SelectedTagIds"]:checked'))
                .map(cb => parseInt(cb.value))
                .filter(n => !isNaN(n));
        }

        return {
            id: id,
            titulo: titulo,
            texto: texto,
            usuarioId: usuarioId,
            selectedTagIds: tagIds
        };
    }

    function showError(container, msg) {
        const box = document.createElement('div');
        box.className = 'alert alert-danger';
        box.textContent = msg;
        const body = container.querySelector('.card-body') || container;
        body.prepend(box);
        setTimeout(() => { box.remove(); }, 5000);
    }

    document.addEventListener('click', function (e) {
        const area = document.getElementById('ajax-area');
        const nova = e.target.closest('#btnNova');
        const editar = e.target.closest('.js-edit');

        if (nova && area) {
            const url = nova.getAttribute('data-url');
            if (url) loadForm(url, area);
            return;
        }
        if (editar && area) {
            const url = editar.getAttribute('data-url');
            if (url) loadForm(url, area);
        }
    });

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', bindIndexButtons);
    } else {
        bindIndexButtons();
    }
})();
