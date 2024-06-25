import http.server
import socketserver
import threading

PORT = 8000
DIRECTORY = ""  # Change this to your directory

class Handler(http.server.SimpleHTTPRequestHandler):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, directory=DIRECTORY, **kwargs)

def start_server():
    with socketserver.TCPServer(("", PORT), Handler) as httpd:
        print(f"Serving at port {PORT}")
        httpd.serve_forever()

# Start the server in a new thread
server_thread = threading.Thread(target=start_server)
server_thread.daemon = True
server_thread.start()


from pythonosc.osc_server import AsyncIOOSCUDPServer
from pythonosc.dispatcher import Dispatcher
from pythonosc import osc_server
import sounddevice as sd
import numpy as np
from scipy.io.wavfile import write

# Global variables to hold audio data and recording state
is_recording = False
recording_data = []
sample_rate = 44100  # Sample rate in Hz

def start_recording(address, *args):
    global is_recording, recording_data
    is_recording = True
    recording_data = []  # Clear previous recording data
    print("Recording started.")

def end_recording(address, *args):
    global is_recording, recording_data
    is_recording = False
    if recording_data:
        # Convert list of numpy arrays to a single numpy array
        audio_data = np.concatenate(recording_data, axis=0)
        # Save as WAV file
        if len(args) > 0:
            filename = args[0] + ".wav"
            write(filename, sample_rate, audio_data)
        else:
            write("output.wav", sample_rate, audio_data)
        print("Recording stopped and saved")
    else:
        print("No audio data recorded.")

def audio_callback(indata, frames, time, status):
    global recording_data
    if is_recording:
        recording_data.append(indata.copy())

# Setup audio input stream
stream = sd.InputStream(callback=audio_callback, channels=1, samplerate=sample_rate, device=1)

# Function to handle OSC messages
def default_handler(address, *args):
    if address == "/startRecording":
        start_recording(address, *args)
    elif address == "/endRecording":
        end_recording(address, *args)

if __name__ == '__main__':
    # OSC setup
    dispatcherosc = Dispatcher()
    dispatcherosc.set_default_handler(default_handler)


    # Create an OSC server thread
    # osc_server = osc_server.BlockingOSCUDPServer(('192.168.137.112', 6161), dispatcherosc)  # Change the IP and port as needed
    osc_server = osc_server.BlockingOSCUDPServer(('192.168.0.115', 6161), dispatcherosc)  # Change the IP and port as needed


    # Start audio stream and OSC server
    with stream:
        print("OSC server running...")
        osc_server.serve_forever()
