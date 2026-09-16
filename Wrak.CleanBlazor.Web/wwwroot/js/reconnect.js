const reconnectModal = document.getElementById('components-reconnect-modal');

if (!reconnectModal) {
    console.warn('Reconnect reconnectModal not found');
} else {

    console.debug('Configuring reconnect state changed listener...');

    let pollingStarted = false;
    let pollTimerId = null;

    const startPolling = () => {
        if (pollingStarted) return;
        pollingStarted = true;

        console.debug('Starting auto-reload polling for server health...');

        async function attemptReload() {
            console.debug('Attempting reload...');
            try {
                const response = await fetch('/health', { cache: 'no-store' });
                if (response.ok) {
                    console.debug('Health check OK, reloading...');

                    // Hide the error UI if it's present before reloading
                    // to prevent it from flashing briefly
                    const errorUi = document.getElementById('blazor-error-ui');
                    if (errorUi) {
                        errorUi.style.display = 'none';
                        errorUi.setAttribute('hidden', 'hidden');
                    }

                    // Navigate to root with reconnected parameter
                    const url = new URL('/', window.location.origin);
                    url.searchParams.set('reloaded', 'true');
                    document.location = url.toString();
                } else {
                    console.debug('Health check returned non-OK:', response.status);
                }
            } catch (err) {
                console.warn('Health check failed:', err);
            }
        }

        // First immediate check, then every 10 seconds
        attemptReload();
        pollTimerId = setInterval(attemptReload, 10000);
    };

    const stopPolling = () => {
        if (pollTimerId !== null) {
            clearInterval(pollTimerId);
            pollTimerId = null;
        }
        pollingStarted = false;
    };

    // Let Blazor tell us when the reconnect modal shows/hides
    reconnectModal.addEventListener('components-reconnect-state-changed', (event) => {
        const state = event.detail?.state;
        console.debug('Reconnect state changed:', state);

        // Only start polling if Blazor has failed to resume the connection
        if (state === 'failed' || state === 'resume-failed' || state === 'rejected') {
            startPolling();
        } else if (state === "show") {
            reconnectModal.showModal();
        } else if (state === "hide") {
            reconnectModal.close();
            stopPolling();
        }
    });
}