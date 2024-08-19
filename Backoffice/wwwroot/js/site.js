const Spinner = {
    html: '<div class="d-flex justify-content-center align-items-center text-light spinner"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>',
    add: (parent = undefined) => {
        if (!parent) parent = 'body'
        $(parent).prepend(Spinner.html)
    },
    remove: () => {
        $('.spinner').remove()
    }
}

$(() => {
    $('form.needs-validation').each((i, f) => {
        const form = $(f)
        form.off('submit')
            .on('submit', e => {
                if (!f.checkValidity()) {
                    e.preventDefault()
                    e.stopPropagation()
                }

                form.addClass('was-validated')
            })
    })
})
