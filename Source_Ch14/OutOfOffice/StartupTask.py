#
# Out of Office sign with mechanical IN/OUT sign
#
# Windows 10 for the Internet of Things (Apress)
# 
# Dr. Charles Bell 
#
import http.server        # Add for html webpage support
import _wingpio as gpio   # Add for GPIO

# Constants for GPIO pins 
SERVO_PIN = 20
AVAIL = 21
DND = 22
BBL = 23
LUNCH = 24
GFTD = 25
LED_PINS = [AVAIL, DND, BBL, LUNCH, GFTD]

#
# Create the HTML for the body of the page post (or get).
# This is a parameterized string to make it easy to modify
# without building the HTML code dynamically. Specifically,
# the following are the parameters in the code.
#
# 0 - "" or disabled to turn input "IN" on/off
# 1 - "" or disabled to turn input "OUT" on/off
# 2 - checked or "" to turn radio button Available on/off
# 3 - checked or "" to turn radio button Do Not Disturb on/off
# 4 - checked or "" to turn radio button Be Back Later on/off
# 5 - checked or "" to turn radio button Out to lunch on/off
# 6 - checked or "" to turn radio button Gone for the day on/off
#
# Initial states are:
#   OUT = disabled
#   Available = checked
#
HTML = """
<html>
    <title>Windows 10 Hardware Control Example</title>
    <body>
        <form method=POST><br>Chuck's Office Status Board: <br>
            <P><input type=submit Value='Set Status to IN ' name='in' {0}/> <br>
            <P><input type=submit Value='Set Status to OUT' name='out' {1}/> <br> <br>
            <input type="radio" name="status" value="avail" {2}> Available<br>
            <input type="radio" name="status" value="dnd" {3}>Do not disturb <br>
            <input type="radio" name="status" value="bbl" {4}> Be back later<br>
            <input type="radio" name="status" value="lunch" {5}> At lunch <br>
            <input type="radio" name="status" value="gftd" {6}> Gone for the day <br>
        </form>
    </body>
</html>
"""

# List of states for the HTML code
states = ["disabled", "", "checked", "", "", "", ""]

# Implement a class for the request handler
class RequestHandler(http.server.BaseHTTPRequestHandler):
    def _set_headers(self):
        # Prepare headers
        self.send_response(200)
        self.send_header('Content-type', 'text/html')
        self.end_headers()

    def do_GET(self):
        # Prepare webpage
        self._set_headers()
        print("Setting states: {0}".format(",".join(states)))
        self.wfile.write(HTML.format(states[0], states[1], states[2], states[3],
                                     states[4], states[5], states[6]).encode())

    def do_HEAD(self):
        # Send header
        self._set_headers()

    def do_POST(self):
        # Get response and repost results
        content_len = int(self.headers.get('content-length', 0))
        post_body = self.rfile.read(content_len)
        option = None
        # reset states for safety
        for i in range(0,7):
            states[i] = ""
        # Turn all pins off
        for i in LED_PINS:
            gpio.output(i, gpio.LOW)
        # Engage the servo
        print("post body = {0}".format(post_body))
        if post_body.startswith(b"in"):
            gpio.output(SERVO_PIN, gpio.LOW)
            states[0] = "disabled"
            states[1] = ""
        # Turn on the LEDS
        elif post_body.startswith(b"out"):
            gpio.output(SERVO_PIN, gpio.HIGH)
            states[0] = ""
            states[1] = "disabled"
        # Need to parse to get the LED switches
        parts = post_body.split(b"&", 1)
        if len(parts) > 1:
            option = parts[1].split(b'=')[1]
        self._set_headers()
        if option:
            if option == b"avail":
                states[2] = "checked"
                gpio.output(AVAIL, gpio.HIGH)
            elif option == b"dnd":
                states[3] = "checked"
                gpio.output(DND, gpio.HIGH)
            elif option == b"bbl":
                states[4] = "checked"
                gpio.output(BBL, gpio.HIGH)
            elif option == b"lunch":
                states[5] = "checked"
                gpio.output(LUNCH, gpio.HIGH)
            elif option == b"gftd":
                states[6] = "checked"
                gpio.output(GFTD, gpio.HIGH)
        print("Setting states: {0}".format(",".join(states)))
        self.wfile.write(HTML.format(states[0], states[1], states[2], states[3],
                                     states[4], states[5], states[6]).encode())

# Initialize the pins, set to LOW (OFF)
for i in LED_PINS:
    gpio.setup(i, gpio.OUT, gpio.PUD_OFF, gpio.HIGH)
    gpio.output(i, gpio.LOW)
gpio.setup(SERVO_PIN, gpio.OUT, gpio.PUD_OFF, gpio.HIGH)
gpio.output(SERVO_PIN, gpio.LOW)

# default is "Available"
gpio.output(AVAIL, gpio.HIGH)

# Run the http server indefinitely
httpd = http.server.HTTPServer(("", 8081), RequestHandler)
print('Started web server on port %d' % httpd.server_address[1])
httpd.serve_forever()
