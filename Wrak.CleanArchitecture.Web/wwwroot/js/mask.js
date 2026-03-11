function applyMaskToElement(elementId, mask, dotNetRef) {
    // notes: apply input mask to elements using the JS-based
    // IMask library to circumvent limitations with
    // MudBlazor's built in PatternMask when using
    // server-side rendering
    // see: https://github.com/MudBlazor/MudBlazor/issues/8186

    const element = document.getElementById(elementId);

    if (element && window.IMask) {
        // initialize IMask
        const maskInstance = IMask(element, { mask: mask });

        // store reference so we can call updateValue later
        element._imaskInstance = maskInstance;

        // add event listener to report changes back to Blazor
        // note: IMask's 'accept' event fires on every valid keystroke,
        // giving you a live value update.
        maskInstance.on('accept', () => {

            if (dotNetRef != null) {
                // call the C# method 'UpdateValue' on the passed Blazor component
                // if one has been provided
                dotNetRef.invokeMethodAsync('UpdateValue', maskInstance.value);
            }
        });
    }
}

function applyDynamicLengthMaskToElement(
    elementId,
    lessThanOrEqualToMask,
    greaterThanMask,
    charThreshold,
    dotNetRef
) {
    const element = document.getElementById(elementId);

    if (element && window.IMask) {
        const maskInstance = IMask(element, {
            // 1. Define mask configs
            mask: [
                { mask: lessThanOrEqualToMask, id: 'less' },
                { mask: greaterThanMask, id: 'greater' }
            ],

            // 2. Choose which compiled mask to use
            dispatch: function (appended, dynamicMasked) {
                const combined = dynamicMasked.unmaskedValue + appended;
                const length = combined.length;
                const masks = dynamicMasked.compiledMasks;

                if (length <= charThreshold) {
                    return masks.find(m => m.id === 'less');
                }

                return masks.find(m => m.id === 'greater');
            }
        });

        // store reference so we can call updateValue later
        element._imaskInstance = maskInstance;

        // Notify Blazor on every valid keystroke
        maskInstance.on("accept", () => {
            if (dotNetRef) {
                dotNetRef.invokeMethodAsync("UpdateValue", maskInstance.value);
            }
        });
    }
}

function syncMaskForElementAndMoveCaret(elementId) {
    const el = document.getElementById(elementId);

    if (!el) return;

    setTimeout(() => {
        const mask = el._imaskInstance;
        if (mask) {
            // Check if mask needs synchronization by comparing unmasked values
            const currentUnmaskedValue = mask.unmaskedValue;
            const inputElement = el.tagName === 'INPUT' ? el : el.querySelector('input');
            
            if (inputElement) {
                // Get the raw input value without mask formatting
                const rawValue = inputElement.value.replace(/\D/g, ''); // Remove non-digits for comparison
                
                // Only update if values are out of sync
                if (rawValue !== currentUnmaskedValue) {
                  console.debug("Mask out of sync - updating mask value...");
                  mask.updateValue();
                  
                  // Move caret to end after update
                  const len = inputElement.value ? inputElement.value.length : 0;
                  inputElement.focus();
                  inputElement.setSelectionRange(len, len);
                } else {
                  console.debug("Mask already in sync - skipping update");
                }
              }
        } else {
            console.warn("Mask instance was null!");
        }
    }, 0);
}