// Register button event
document.getElementById("register").addEventListener("click", async () => {
    try {
        const response = await fetch(
            "https://localhost:7296/register-challenge"
        );
        const { challenge, user } = await response.json();

        // Start registration process
        const credential = await navigator.credentials.create({
            publicKey: {
                challenge: base64ToArrayBuffer(challenge),
                rp: { name: "Biometric Example" },
                user: {
                    id: Uint8Array.from(user.id, (c) => c.charCodeAt(0)),
                    name: user.name,
                    displayName: user.displayName,
                },
                pubKeyCredParams: [
                    {
                        type: "public-key",
                        alg: -7, // "ES256" as registered in the IANA COSE Algorithms registry
                    },
                ],
                authenticatorSelection: {
                    authenticatorAttachment: "platform", // Use built-in fingerprint sensor
                    userVerification: "required",
                },
                timeout: 60000,
            },
        });

        const data = {
            id: credential.id,
            rawId: arrayBufferToBase64(credential.rawId),
            type: credential.type,
            response: {
                clientDataJSON: arrayBufferToBase64(credential.response.clientDataJSON),
                attestationObject: arrayBufferToBase64(
                    credential.response.attestationObject
                ),
            },
        };

        await fetch("https://localhost:7296/register", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(data),
        });

        alert("Registration successful!");
    } catch (error) {
        console.error("Registration failed", error);
    }
});

// Verify button event
document.getElementById("verify").addEventListener("click", async () => {
    try {
        const response = await fetch("https://localhost:7296/verify-challenge");
        const { challenge, allowCredentials } = await response.json();

        // Start verify process
        const assertion = await navigator.credentials.get({
            publicKey: {
                challenge: base64ToArrayBuffer(challenge),
                allowCredentials: allowCredentials.map((cred) => ({
                    id: base64ToArrayBuffer(cred.id),
                    type: "public-key",
                })),
                userVerification: "required",
                timeout: 60000,
            },
        });

        const data = {
            id: assertion.id,
            rawId: arrayBufferToBase64(assertion.rawId),
            type: assertion.type,
            response: {
                clientDataJSON: arrayBufferToBase64(assertion.response.clientDataJSON),
                authenticatorData: arrayBufferToBase64(
                    assertion.response.authenticatorData
                ),
                signature: arrayBufferToBase64(assertion.response.signature),
                userHandle: assertion.response.userHandle
                    ? arrayBufferToBase64(assertion.response.userHandle)
                    : null,
            },
        };

        const verifyResponse = await fetch(
            "https://localhost:7296/verify",
            {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(data),
            }
        );

        if (verifyResponse.ok) {
            alert("Verification successful!");
        } else {
            alert("Verification failed!");
        }
    } catch (error) {
        console.error("Verification failed", error);
    }
});


// Base64 to ArrayBuffer helper
function base64ToArrayBuffer(base64) {
    const binaryString = atob(base64.replace(/-/g, "+").replace(/_/g, "/"));
    const len = binaryString.length;
    const bytes = new Uint8Array(len);
    for (let i = 0; i < len; i++) {
        bytes[i] = binaryString.charCodeAt(i);
    }
    return bytes.buffer;
}

function convertArr(arrayBuffer) {
    // Convert the ArrayBuffer to a binary string
    let binary = "";
    const bytes = new Uint8Array(arrayBuffer);
    for (let i = 0; i < bytes.length; i++) {
        binary += String.fromCharCode(bytes[i]);
    }

    // Encode the binary string to Base64
    return btoa(binary);
}

// ArrayBuffer to Base64 helper
function arrayBufferToBase64(buffer) {
    const bytes = new Uint8Array(buffer);
    let binary = "";
    for (let i = 0; i < bytes.byteLength; i++) {
        binary += String.fromCharCode(bytes[i]);
    }
    return btoa(binary)
        .replace(/\+/g, "-")
        .replace(/\//g, "_")
        .replace(/=+$/, "");
}